using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using distribution_copy.Models;
using Newtonsoft.Json;
using Newtonsoft;
using System.IO;
using System.Web.Script.Serialization;

namespace distribution_copy.Controllers
{
    public class AccountController : Controller
    {
        public string url = "";
        // GET: Account
        public ActionResult Verify(LoginModel model)
        {
            return View(model);
        }

        //[HttpGet]
        //[AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                Session["visited"] = "1";
                string url = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state=User1&scope={1}&redirect_uri={2}";
                string redirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
                string AppScope = System.Configuration.ConfigurationManager.AppSettings["appScope"];
                url = string.Format(url, clientId, AppScope, redirectUrl);
                return Redirect(url);
            }
            catch (Exception ex)
            {
                //logger.Debug(JsonConvert.SerializeObject(ex, Formatting.Indented) + Environment.NewLine);
            }
            return RedirectToAction("../shared/error");
        }
        public JsonResult AccountList()
        {
            AccountsResponse.AccountList accountList = new AccountsResponse.AccountList();
            if (Session["AccountList"] != null)
            {
                accountList = (AccountsResponse.AccountList)Session["AccountList"];
            }
            return Json(accountList.value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProjectList(string ORG)
        {
            string responseBody = "";
            ProjectModel pm = new ProjectModel();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", Session["PAT"] == null ? Request.QueryString["code"] : Session["PAT"].ToString()))));

                    using (HttpResponseMessage response = client.GetAsync("https://dev.azure.com/" + ORG + "/_apis/projects?api-version=5.1").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        responseBody = response.Content.ReadAsStringAsync().Result;
                    }

                }
                pm = JsonConvert.DeserializeObject<ProjectModel>(responseBody);
            }
            catch (Exception ex)
            {
                return Json("");
            }

            return Json(pm.Value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult WITypes(InputModel inp)
        {
            ResponseWI wiqlResponse = new ResponseWI();

            ResponseWI urlResponse = new ResponseWI();
            string responseBody = "";
            string queryString = @"Select [Work Item Type],[State], [Title],[Created By] From WorkItems ";

            queryString += "Order By [Stack Rank] Desc, [Backlog Priority] Desc";
            var wiql = new
            {
                query = queryString
            };
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", Session["PAT"] == null ? Request.QueryString["code"] : Session["PAT"].ToString()))));
                var request = new HttpRequestMessage(
                    new HttpMethod("POST"),
                    "https://dev.azure.com/" + inp.OrganizationName + "/_apis/wit/wiql?api-version=5.1"
                    )
                { Content = content };
                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    responseBody = response.Content.ReadAsStringAsync().Result;
                }
                wiqlResponse = JsonConvert.DeserializeObject<ResponseWI>(responseBody);
            }

            if (wiqlResponse.workItems == null || wiqlResponse.workItems.Count == 0)
                return null;
            string defaultUrl = "https://dev.azure.com/" + inp.OrganizationName + "/_apis/wit/workitems?ids=";
            url = defaultUrl;
            urlResponse.value = new List<Value>();
            string b = "&api-version=5.1";
            for (int j = 0; j < wiqlResponse.workItems.Count; j++)
            {
                if (j % 200 == 0 && j != 0)
                {

                    var batchResponse = getWorkItems(inp);
                    urlResponse.count += batchResponse.count;
                    foreach (var item in batchResponse.value)
                    {
                        urlResponse.value.Add(item);
                    }
                    url = defaultUrl;
                }
                if (j % 200 == 0)
                {
                    url += wiqlResponse.workItems[j].id;
                }
                else
                {
                    url += "," + wiqlResponse.workItems[j].id;
                }
            }
            url += b;

            var lastBatchResponse = getWorkItems(inp);
            urlResponse.count += lastBatchResponse.count;
            foreach (var item in lastBatchResponse.value)
            {
                urlResponse.value.Add(item);
            }
            Session["WorkItems"] = urlResponse;
            List<string> Types = new List<string>();
            foreach (var i in urlResponse.value)
            {
                if (!Types.Contains(i.fields.WorkItemType))
                    Types.Add(i.fields.WorkItemType);
            }
            return Json(Types, JsonRequestBehavior.AllowGet);
        }
        public ResponseWI getWorkItems(InputModel inp)
        {
            ResponseWI batch;
            string responseBody2 = "";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", Session["PAT"] == null ? Request.QueryString["code"] : Session["PAT"].ToString()))));

                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        responseBody2 += response.Content.ReadAsStringAsync().Result;
                        batch = JsonConvert.DeserializeObject<ResponseWI>(responseBody2);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return batch;

        }
        public JsonResult getWorkItembyType(InputModel inp)
        {
            ResponseWI Witems = new ResponseWI();
            if (Session["WorkItems"] == null)
                WITypes(inp);
            Witems = (ResponseWI)Session["WorkItems"];
            var result = Json(Witems.value.Where(x => x.fields.WorkItemType == inp.WorkItemType.Trim()), JsonRequestBehavior.AllowGet);
            return result;
        }
        public JsonResult AllList(InputModel inp)
        {
            ResponseWI Witems = new ResponseWI();
            if (Session["WorkItems"] == null)
                WITypes(inp);

            List<string> WorkItemList = new List<string>();
            List<string> AssignedToList = new List<string>();
            List<string> SprintList = new List<string>();
            List<string> StateList = new List<string>();
            foreach (var i in ((ResponseWI)Session["WorkItems"]).value.Where(x => x.fields.TeamProject == inp.ProjectName))
            {

                if (i.fields.AssignedTo != null)
                {
                    if (i.fields.AssignedTo.uniqueName != null)
                    {
                        if (!AssignedToList.Contains(i.fields.AssignedTo.uniqueName))
                            AssignedToList.Add(i.fields.AssignedTo.uniqueName);
                    }
                    else
                    {
                        if (!AssignedToList.Contains(i.fields.AssignedTo.displayName))
                            AssignedToList.Add(i.fields.AssignedTo.displayName);
                    }
                }
                if (i.fields.Sprint != null)
                {
                    if (!SprintList.Contains(i.fields.Sprint))
                        SprintList.Add(i.fields.Sprint);
                }
                if (i.fields.State != null)
                {
                    if (!StateList.Contains(i.fields.State))
                        StateList.Add(i.fields.State);
                }
                if (i.fields.WorkItemType != null)
                {
                    if (!WorkItemList.Contains(i.fields.WorkItemType))
                        WorkItemList.Add(i.fields.WorkItemType);
                }
            }
            List<List<string>> all = new List<List<string>>();
            all.Add(AssignedToList);
            all.Add(SprintList);
            all.Add(StateList);
            all.Add(WorkItemList);
            return Json(all, JsonRequestBehavior.AllowGet);
        }

        public object Filter(InputModel inp,int loc)
        {

            ResponseWI wI = (ResponseWI)Session["WorkItems"];
            ResponseWI returnWI = new ResponseWI();
            if (inp.ProjectName != null&&inp.ProjectName!= "Empty List"&&inp.ProjectName!="0")
            {
                returnWI.value = new List<Value>();
                foreach (var i in wI.value)
                {
                    if (i.fields.TeamProject == inp.ProjectName)
                        returnWI.value.Add(i);
                }
            }
            else
            {
                returnWI = wI;
            }

            ResponseWI returnWI2 = new ResponseWI();
            if (inp.WorkItemType != null && inp.WorkItemType != "Empty List"&& inp.WorkItemType !="0")
            {
                returnWI2.value = new List<Value>();
                foreach (var i in returnWI.value)
                {
                    if (i.fields.WorkItemType == inp.WorkItemType)
                        returnWI2.value.Add(i);
                }
            }
            else
                returnWI2 = returnWI;

            ResponseWI returnWI3 = new ResponseWI();

            if (inp.AssignedTo != null && inp.AssignedTo != "Empty List" && inp.AssignedTo != "0")
            {
                returnWI3.value = new List<Value>();
                foreach (var i in returnWI2.value)
                {
                    if (i.fields.AssignedTo != null)
                    {
                        if (i.fields.AssignedTo.uniqueName == inp.AssignedTo|| i.fields.AssignedTo.displayName ==  inp.AssignedTo)
                            returnWI3.value.Add(i);
                    }
                }
            }
            else
                returnWI3 = returnWI2;

            ResponseWI returnWI4 = new ResponseWI();

            if (inp.Sprint != null && inp.Sprint != "Empty List" && inp.Sprint != "0")
            {
                returnWI4.value = new List<Value>();
                foreach (var i in returnWI3.value)
                {
                    if (i.fields.Sprint == inp.Sprint)
                        returnWI4.value.Add(i);
                }
            }
            else
                returnWI4 = returnWI3;

            ResponseWI returnWI5 = new ResponseWI();

            if (inp.State != null && inp.State != "Empty List" && inp.State != "0")
            {
                returnWI5.value = new List<Value>();
                foreach (var i in returnWI4.value)
                {
                    if (i.fields.State == inp.State)
                        returnWI5.value.Add(i);
                }
            }
            else
                returnWI5 = returnWI4;

            ResponseWI FilteredWI = new ResponseWI();

            if (inp.CreatedDate != null && inp.CreatedDate != "")
            {
                FilteredWI.value = new List<Value>();
                foreach (var i in returnWI5.value)
                {
                    if (i.fields.CreatedDate.Date >= DateTime.Parse(inp.CreatedDate))
                        FilteredWI.value.Add(i);
                }
            }
            else
                FilteredWI = returnWI5;
            if (loc > 0)
                return FilteredWI;
            else
            {
                string output = JsonConvert.SerializeObject(FilteredWI.value);
                //return Json(FilteredWI.value, JsonRequestBehavior.AllowGet);
                return output;
            }

        }
        public ActionResult SignOut()
        {
            Session.Clear();
       return Redirect("https://app.vssps.visualstudio.com/_signout");
        }

        public ActionResult Export(InputModel inp)
        {
            //var data = .Data;
            ResponseWI response = new ResponseWI();
            response.value = new List<Value>();
            response = (ResponseWI)Filter(inp, 1);
            //response.value = JsonConvert.DeserializeObject<List<Value>>(new JavaScriptSerializer().Serialize(Filter(inp,1).Data));
            generateExcel(response, inp);
            return RedirectToAction("../WIReport/Index");
        }
        public void generateExcel(ResponseWI wi, InputModel inp)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("WorkItems");
            workSheet.TabColor = System.Drawing.Color.White;
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Cells[1, 1].Value = "ID";
            workSheet.Cells[1, 2].Value = "Work Item Type";
            workSheet.Cells[1, 3].Value = "Title";
            workSheet.Cells[1, 4].Value = "Team Project";
            workSheet.Cells[1, 5].Value = "Assigned To";
            workSheet.Cells[1, 6].Value = "State";
            workSheet.Cells[1, 7].Value = "Url";
            workSheet.Cells[1, 8].Value = "PlannedHours";
            workSheet.Cells[1, 9].Value = "ActualHours";
            workSheet.Cells[1, 10].Value = "Sprint";
            workSheet.Cells[1, 11].Value = "OriginalEstimate";
            workSheet.Cells[1, 12].Value = "CompletedWork";
            workSheet.Cells[1, 13].Value = "RemainingWork";
            workSheet.Cells[1, 14].Value = "CreatedDate";
            workSheet.Cells[1, 15].Value = "Description";
            workSheet.Cells[1, 16].Value = "CreatedBy";
            workSheet.Cells[1, 17].Value = "AssignedTo";
            workSheet.Cells[1, 18].Value = "ChangedBy";
            int recordIndex = 2;
            int columnNo = 0;
            foreach (var WI in wi.value)
            {
                columnNo = 0;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.id;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.WorkItemType;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.Title;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.TeamProject;
                if (WI.fields.AssignedTo != null)
                    workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.AssignedTo.displayName;
                else
                    workSheet.Cells[recordIndex, ++columnNo].Value = "Unassigned";
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.State;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.url;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.PlannedHours;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.ActualHours;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.Sprint;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.OriginalEstimate;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.CompletedWork;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.RemainingWork;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.CreatedDate.ToShortDateString();
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.Description;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.CreatedBy == null ? "" : WI.fields.CreatedBy.displayName;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.AssignedTo == null ? "" : WI.fields.AssignedTo.displayName;
                workSheet.Cells[recordIndex, ++columnNo].Value = WI.fields.ChangedBy == null ? "" : WI.fields.ChangedBy.displayName;
                recordIndex++;

            }
            for (var i = 1; i <= columnNo; i++)
                workSheet.Column(i).AutoFit();
            string excelName = inp.OrganizationName + "-" + (inp.ProjectName != null ? inp.ProjectName : "" )+ "-" + (inp.WorkItemType != null ? inp.WorkItemType : "" )+ DateTime.Now.ToString();
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
    }
}