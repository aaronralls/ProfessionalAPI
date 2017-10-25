using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace ProfessionalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                //Remove the server header to not expose the server type and framework.
                //https://jeremylindsayni.wordpress.com/2016/12/22/creating-a-restful-web-api-template-in-net-core-1-1-part-4-securing-the-service-against-xss-clickjacking-and-drive-by-downloads/
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
