using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProfessionalAPI2.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        
        /// <summary>
        /// GET api/values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = new string[] { "value1", "value2" };

            AddPagingMetaData(items.Count().ToString());

            return new OkObjectResult(items);
        }

        /// <summary>
        /// //https://stackoverflow.com/questions/3715981/what-s-the-best-restful-method-to-return-total-number-of-items-in-an-object
        /// </summary>
        /// <param name="itemCount"></param>
        private void AddPagingMetaData(string itemCount)
        {
            Response.Headers.Add("x-total-count", itemCount);
        }

        
        /// <summary>
        /// GET api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            AddPagingMetaData("1");
            return new OkObjectResult("value");
        }

        
        /// <summary>
        /// POST api/values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            return new CreatedResult("api/Values/3", "value3");
        }

        /// <summary>
        /// PUT api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]string value)
        {
            return Accepted(value);
        }

        /// <summary>
        /// DELETE api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return NoContent();
        }
    }
}
