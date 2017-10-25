using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc.Formatters;
using NetEscapades.AspNetCore.SecurityHeaders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProfessionalAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            //test your API here: https://securityheaders.io/ or
            //https://www.htbridge.com/websec/
            services.AddCustomHeaders();

            //https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?tabs=aspnetcore1x
            //https://www.janaks.com.np/enabling-gzip-compression-asp-net-core-1-1-application/
            //http://checkgzipcompression.com/faq/how-to-enable-gzip-compression/
            //https://jeremylindsayni.wordpress.com/2016/11/29/creating-a-restful-web-api-template-in-net-core-1-1-part-3-improving-the-performance-by-using-compression/
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();                
            });

            // Add essential framework services.
            services.AddMvcCore(config =>
                    {
                        // Add XML Content Negotiation
                        //https://wildermuth.com/2016/03/16/Content_Negotiation_in_ASP_NET_Core
                        config.RespectBrowserAcceptHeader = true;
                        config.InputFormatters.Add(new XmlSerializerInputFormatter());
                        config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                    })
                    //Uses attribute routing
                    //https://andrewlock.net/introduction-to-the-apiexplorer-in-asp-net-core/
                    //Attribute Routing(WEB APIs) vs Convention Routing(MVC Controllers)
                    //https://exceptionnotfound.net/attribute-routing-vs-convention-routing/
                    .AddApiExplorer()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddJsonOptions(options =>
                    {
                        // handle loops correctly
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        // include $id property in the output
                        options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                        options.SerializerSettings.Formatting = Formatting.Indented;
                        // use standard name conversion of properties Force Camel Case to JSON
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    })
                    .AddCors(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);

            app.UseResponseCompression();
            
            app.UseCustomHeadersMiddleware(SetHeaderPolicy());

            app.UseCors("AllowAll");

            app.UseMvcWithDefaultRoute();
        }

        private HeaderPolicyCollection SetHeaderPolicy()
        {
            return new HeaderPolicyCollection()
                    .AddFrameOptionsSameOrigin()    // prevent click-jacking
                    .AddXssProtectionBlock()    // prevent cross-site scripting (XSS)
                    .AddContentTypeOptionsNoSniff() // prevent drive-by-downloads
                    //https://scotthelme.co.uk/a-new-security-header-referrer-policy/
                    .AddCustomHeader("Referrer-Policy", "no-referrer-when-downgrade")
                    //TODO Tighten this up
                    // https://content-security-policy.com/
                    .AddCustomHeader("Content-Security-Policy-Report-Only", "default-src 'unsafe-inline' ;  child-src 'unsafe-inline' ; script-src 'unsafe-inline' ;  child-src 'unsafe-inline' ; connect-src 'self'; img-src 'self'; style-src 'unsafe-inline'  ; child-src 'unsafe-inline'"); 
        }
    }
}
