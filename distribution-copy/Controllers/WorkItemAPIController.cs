using distribution_copy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace distribution_copy.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WorkItemAPIController : ApiController
    {
        
        public IHttpActionResult GetOrganiaztions()
        {
           
            return Ok();
        }  
        
    }
}
