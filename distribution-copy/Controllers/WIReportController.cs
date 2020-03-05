using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using distribution_copy.Models;
using distribution_copy.Services;

namespace distribution_copy.Controllers
{
    public class WIReportController : Controller
    {
        AccountService Account = new AccountService();
        // GET: WIReport
        public ActionResult Index()
        {
            if (Session["visited"]==null)
            {
                return RedirectToAction("../Account/Verify");
            }
            if (Session["PAT"] == null) { 
            InputModel input = new InputModel();
            try
            {

                AccessDetails _accessDetails = new AccessDetails();

                AccountsResponse.AccountList accountList = null;
                string code = Session["PAT"] == null ? Request.QueryString["code"] : Session["PAT"].ToString();
                string redirectUrl = ConfigurationManager.AppSettings["RedirectUri"];
                string clientId = ConfigurationManager.AppSettings["ClientSecret"];
                string accessRequestBody = string.Empty;
                accessRequestBody = Account.GenerateRequestPostData(clientId, code, redirectUrl);
                _accessDetails = Account.GetAccessToken(accessRequestBody);
                    ProfileDetails profile = Account.GetProfile(_accessDetails);

                    if (!string.IsNullOrEmpty(_accessDetails.access_token))
                {
                    Session["PAT"] = _accessDetails.access_token;
                 
                    if (profile.displayName != null || profile.emailAddress != null)
                    {
                        Session["User"] = profile.displayName ?? string.Empty;
                        Session["Email"] = profile.emailAddress ?? profile.displayName.ToLower();
                    }
                }
                    accountList = Account.GetAccounts(profile.id, _accessDetails);
                    Session["AccountList"] = accountList;
                    string pat = Session["PAT"].ToString();
                List<SelectListItem> OrganizationList = new List<SelectListItem>();
                foreach (var i in accountList.value)
                {
                    OrganizationList.Add(new SelectListItem { Text = i.accountName, Value = i.accountName });
                }
                ViewBag.OrganizationList = OrganizationList;
              


            }
            catch (Exception ex)
            {



            }
            }

            return View();


        }




        // GET: WIReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WIReport/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WIReport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WIReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WIReport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WIReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WIReport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
