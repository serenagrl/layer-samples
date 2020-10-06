using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using LeaveSample.Entities;
using LeaveSample.UI.Process;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using LeaveSample.UI.Web.SignalR;

namespace LeaveSample.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        public const int MAXIMUM_ROWS = 10;

        private IHubConnectionContext<ILeaveActionClient> _clients = 
            GlobalHost.ConnectionManager.GetHubContext<LeaveActionHub, ILeaveActionClient>().Clients;

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Categories = GetCategories();

            var leave = new Leave();
            leave.Employee = Environment.UserName;
            leave.StartDate = DateTime.Now;
            leave.EndDate = DateTime.Now;
            leave.Category = LeaveCategories.Annual;
            leave.Duration = 1;

            return View(leave);
        }

        [HttpPost]
        public ActionResult Index(Leave leave)
        {
            ViewBag.Categories = GetCategories();
            
            // Validate date.
            if (leave.StartDate > leave.EndDate)
            {
                ModelState.AddModelError("DateError",
                    "Start date cannot be greater than End date.");
            }
            else
            {
                // Validate duration.
                var totalDays = leave.EndDate.Subtract(leave.StartDate).Days + 1;
                if (leave.Duration > totalDays)
                    ModelState.AddModelError("DurationError",
                        "Duration cannot be greater than the number of days in selected date range.");
            }

            leave.Status = LeaveStatuses.Pending;

            if (ModelState.IsValid)
            {
                try
                {
                    var upc = new LeaveProcessComponent();
                    leave = upc.Apply(leave);
                    _clients.All.LeaveApplied(leave);
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(leave);
        }

        private string[] GetCategories()
        {
            return Enum.GetNames(typeof(LeaveCategories));
        }

        public static List<SelectListItem> CreateListItems(Type type)
        {
            int i = 0;
            List<SelectListItem> list = Enum.GetNames(type)
                .Select(v => new SelectListItem() { Text = v, Value=i++.ToString() } ).ToList();

            list.Insert(0, new SelectListItem() { Text = "- All -", Value="" });

            return list;
        }

        public ActionResult Leaves(
            LeaveCategories? categoryFilter = null,
            LeaveStatuses? statusFilter = null,
            int page = 1, string sort = "DateSubmitted", string sortDir = "DESC")
        {
            List<Leave> leaves = ListLeaves(categoryFilter, statusFilter, Environment.UserName, page, sort, sortDir);
            return View(leaves);
        }

        [HttpGet]
        public ActionResult Approvals(
            LeaveCategories? category = null, 
            LeaveStatuses? status = null, 
            int page = 1, string sort = "DateSubmitted", string sortDir = "DESC")
        {
            List<Leave> leaves = ListLeaves(category, status, null, page, sort, sortDir);
            return View(leaves);
        }

        private List<Leave> ListLeaves(LeaveCategories? category, LeaveStatuses? status, 
            string employee, int page, string sort, string sortDir)
        {
            var categories = CreateListItems(typeof(LeaveCategories));
            var statuses = CreateListItems(typeof(LeaveStatuses));

            // Get selected Category.
            if (category.HasValue)
                categories.Find(x => x.Text == category.Value.ToString()).Selected = true;

            // Get selected Status.
            if (status.HasValue)
                statuses.Find(x => x.Text == status.Value.ToString()).Selected = true;

            ViewBag.CategoryList = categories;
            ViewBag.StatusList = statuses;

            int totalRowCount = 0;
            List<Leave> leaves = new List<Leave>();

            int startRowIndex = (page - 1) * MAXIMUM_ROWS;

            var upc = new LeaveProcessComponent();
            leaves = upc.ListLeavesByEmployee(MAXIMUM_ROWS, startRowIndex,
                string.Format("{0} {1}", sort, sortDir), employee,
                category, status, out totalRowCount);

            ViewBag.TotalRowCount = totalRowCount;
            return leaves;
        }

        [HttpPost]
        public ActionResult Cancel(Leave leave)
        {
            if (ModelState.IsValid)
            {
                var upc = new LeaveProcessComponent();
                leave = upc.Cancel(leave);
                _clients.All.LeaveCancelled(leave);
            }

            return GetJsonResult(leave);
        }

        [HttpPost]
        public ActionResult Approve(Leave leave)
        {
            if (ModelState.IsValid)
            {
                var upc = new LeaveProcessComponent();
                leave = upc.Approve(leave);
                _clients.All.LeaveUpdated(leave);
            }

            return GetJsonResult(leave);
        }

        [HttpPost]
        public ActionResult Reject(Leave leave)
        {
            if (ModelState.IsValid)
            {
                var upc = new LeaveProcessComponent();
                leave = upc.Reject(leave);
                _clients.All.LeaveUpdated(leave);
            }

            return GetJsonResult(leave);
        }

        private static ContentResult GetJsonResult(Leave leave)
        {
            var result = new ContentResult();
            result.Content = JsonConvert.SerializeObject(leave, new StringEnumConverter());
            result.ContentType = "application/json";
            return result;
        }

    }
}
