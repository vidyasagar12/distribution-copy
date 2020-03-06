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
        //[HttpPost]
        //public IQueryable<Value> Filtered(InputModel inp, int loc)
        //{
        //    AccountController account = new AccountController();
        //    var data = (List<Value>)account.Filter(inp, loc);
        //    return data.AsQueryable();
        //}
        //[HttpPost]
        //public IQueryable<Value> getWorkItembyTypeApi(InputModel inp)
        //{
        //    //AccountController account = new AccountController();
        //    //var data = (List<Value>)account.getWorkItembyType(inp);
        //    //return data.AsQueryable();
        //}
    }
}
