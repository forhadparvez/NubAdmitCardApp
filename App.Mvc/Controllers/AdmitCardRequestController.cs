using App.Core.Application;
using App.Core.Command;
using App.Core.Entities;
using App.Mvc.Models;
using System.Linq;
using System.Web.Mvc;

namespace App.Mvc.Controllers
{
    public class AdmitCardRequestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdmitCardRequest
        public ActionResult Create()
        {
            ViewBag.Message = "";
            ViewBag.MessageColor = "";
            return View();
        }

        [HttpPost]
        public ActionResult Create(AdmitCardRequestCommand command)
        {

            if (ModelState.IsValid)
            {


                var s = db.StudentInfos.SingleOrDefault(c => c.IdNo == command.IdNo);
                if (s != null)
                {
                    var ar = db.AdmitCardRequests.Any(c => c.StudentInfoId == s.Id && !c.IsDone);
                    if (ar)
                    {
                        ViewBag.Message = "You Already Have Pending Request";
                        ViewBag.MessageColor = "text-danger";
                        return View(command);
                    }
                    var a = new AdmitCardRequest()
                    {
                        StudentInfoId = s.Id,
                        RequestedDate = DateTimeFormatter.StringToDate(command.RequestedDate),
                        Comment = command.Comment
                    };

                    db.AdmitCardRequests.Add(a);
                    var r = db.SaveChanges();

                    if (r > 0)
                    {
                        ViewBag.Message = "Successfully Submitted";
                        ViewBag.MessageColor = "text-success";
                        return View();
                    }

                    ViewBag.Message = "Submit Fail";
                    ViewBag.MessageColor = "text-danger";
                    return View(command);
                }
                else
                {
                    ViewBag.Message = "ID Not Found";
                    ViewBag.MessageColor = "text-warning";
                }

            }
            ViewBag.Message = "Submit Valid Value";
            ViewBag.MessageColor = "text-warning";
            return View(command);
        }
    }
}