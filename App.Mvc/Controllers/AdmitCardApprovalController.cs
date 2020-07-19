using App.Core.Entities;
using App.Mvc.Models;
using System;
using System.Web.Mvc;

namespace App.Mvc.Controllers
{
    public class AdmitCardApprovalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: AdmitCardApproval
        public JsonResult GiveApproval(long stdId)
        {
            var a = new AdmitCardApproval()
            {
                StudentInfoId = stdId,
                IsPaymentComplete = true,
                ApproveBy = "",
                ApproveDate = DateTime.Now
            };

            db.AdmitCardApprovals.Add(a);
            var r = db.SaveChanges();
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}