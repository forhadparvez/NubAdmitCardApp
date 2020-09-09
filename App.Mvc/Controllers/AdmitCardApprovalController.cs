using App.Core.Application;
using App.Core.Entities;
using App.Mvc.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace App.Mvc.Controllers
{
    [Authorize]
    public class AdmitCardApprovalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: AdmitCardApproval
        public JsonResult GiveApproval(long stdId)
        {
            var user = User.Identity.Name;

            var li = db.AdmitCardApprovals.Where(c => c.StudentInfoId == stdId);
            foreach (var ap in li)
            {
                ap.IsPrevious = true;
            }
            db.SaveChanges();

            var a = new AdmitCardApproval()
            {
                StudentInfoId = stdId,
                IsPaymentComplete = true,
                IsPrevious = true,
                ApproveBy = user,
                ApproveDate = DateTime.Now
            };

            db.AdmitCardApprovals.Add(a);
            var r = db.SaveChanges();
            return Json(r, JsonRequestBehavior.AllowGet);
        }



        public JsonResult DueApproval(long stdId, long? requestId, string date, long paymentId)
        {
            var li = db.AdmitCardApprovals.Where(c => c.StudentInfoId == stdId);
            foreach (var ap in li)
            {
                ap.IsPrevious = true;
            }
            db.SaveChanges();

            var stds = db.AdmitCardRequests.Where(c => c.StudentInfoId == stdId);
            foreach (var std in stds)
            {
                std.IsDone = true;
                std.Status = true;
            }
            db.SaveChanges();

            var user = User.Identity.Name;
            var a = new AdmitCardApproval()
            {
                PaymentId = paymentId,
                StudentInfoId = stdId,
                ExceptedDate = DateTimeFormatter.StringToDate(date),
                IsSpecialPermission = true,
                Comments = "",
                IsPrevious = false,
                ApproveBy = user,
                ApproveDate = DateTime.Now
            };

            db.AdmitCardApprovals.Add(a);
            var r = db.SaveChanges();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

    }
}