using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace distribution_copy.Controllers
{
    public class WorkItemAPIController : ApiController
    {
        
        public IHttpActionResult GetOrganiaztions()
        {
           
            return Ok();
        }
    }
}
