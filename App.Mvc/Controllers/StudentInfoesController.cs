using App.Core.Application;
using App.Core.Command;
using App.Core.Entities;
using App.Core.Query;
using App.Mvc.Models;
using App.Mvc.ViewModels;
using MathCaptcha;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Mvc.Controllers
{
    [Authorize]
    public class StudentInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly Captcha captcha = new Captcha();

        // GET: StudentInfoes
        public async Task<ActionResult> ApprovedList()
        {
            var stdresult = new List<StudentInfo>();

            var studentInfos = await db.StudentInfos.Include(s => s.Program).Where(c => !c.IsDelete).ToListAsync();
            foreach (var std in studentInfos)
            {
                var isExist = db.AdmitCardApprovals.Any(c => c.StudentInfoId == std.Id && !c.IsDelete);
                if (isExist)
                    stdresult.Add(std);
            }

            return View(stdresult);
        }

        public ActionResult PaymentStatusApproval()
        {
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            return View();
        }

        public async Task<ActionResult> GetPaymentStatusApproval(byte programId, int semesterId, byte examId)
        {
            string root = Server.MapPath("~");
            var outputPath = root + @"StudentInfoes";
            if (Directory.Exists(outputPath))
            {
                var di = new DirectoryInfo(outputPath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(outputPath);
            }

            var result = new List<StudentInfo>();

            var payments = await db.Payments.Include(s => s.Program)
                .Where(c => !c.IsDelete && c.ProgramId == programId && c.SemesterId == semesterId && c.ExamId == examId).ToListAsync();
            foreach (var pay in payments)
            {
                var isExist = db.AdmitCardApprovals.Any(c => c.PaymentId == pay.Id && !c.IsDelete);
                var std = await db.StudentInfos.Include(c => c.Program).SingleOrDefaultAsync(c => c.IdNo == pay.StudentId);
                var isExist2 = db.AdmitCardRequests.Any(c => c.StudentInfoId == std.Id && !c.IsDone);

                if (!isExist || isExist2)
                    result.Add(std);

            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public async Task<JsonResult> GetDetails(long stdId, byte programId, int semesterId, byte examId)
        {
            string root = Server.MapPath("~");
            var outputPath = root + @"StudentInfoes";
            if (Directory.Exists(outputPath))
            {
                var di = new DirectoryInfo(outputPath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(outputPath);
            }



            var std = await db.StudentInfos.Include(s => s.Program).SingleOrDefaultAsync(c => !c.IsDelete && c.Id == stdId);

            if (std != null)
            {
                var result = new PaymentStatusApprovalQuery()
                {
                    Id = std.Id,
                    IdNo = std.IdNo,
                    Name = std.Name
                };

                var payments = await db.Payments.Include(s => s.Program)
                    .FirstOrDefaultAsync(c => !c.IsDelete && c.ProgramId == programId && c.SemesterId == semesterId && c.ExamId == examId && c.StudentId == std.IdNo);

                if (payments != null)
                {
                    result.PaymentStatus = payments.DuesPercentAmount.ToString();
                    result.PaymentId = payments.Id;
                }


                var request = await db.AdmitCardRequests.SingleOrDefaultAsync(c => !c.IsDone && c.StudentInfoId == std.Id);
                if (request != null)
                {
                    result.RequestId = request.Id.ToString();
                    result.RequestedDate = DateTimeFormatter.DateToString(request.RequestedDate);
                    result.Comment = request.Comment;
                }

                //var link = "D" + std.PaymentFilePath;
                //var sourchFile = Path.Combine(link);
                //var tergetPath = Path.Combine(root + @"\StudentInfoes", std.Id + ".jpg");

                //System.IO.File.Copy(sourchFile, tergetPath, true);
                //result.PaymentFilePath = std.Id + ".jpg";


                var request2 = await db.AdmitCardRequests.Where(c => c.IsDone && c.Status && c.StudentInfoId == std.Id).ToListAsync();
                foreach (var r in request2)
                {
                    result.PreviousPermission += "Till: " + DateTimeFormatter.DateToString(r.RequestedDate) + ", ";
                }


                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(0, JsonRequestBehavior.AllowGet);
        }




        [AllowAnonymous]
        public ActionResult AdmitDownload()
        {
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            return View();
        }


        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult AdmitDownload(AdmitCardRequestCommand vm)
        {

            CultureInfo cInfo = new CultureInfo("en-IN");
            ReportViewer viewer = new ReportViewer();



            var context = new ApplicationDbContext();

            // data source
            var db = new ApplicationDbContext();

            var idNo = vm.IdNo;

            var s = db.StudentInfos.Include(c => c.Program)
                .SingleOrDefault(c => c.IdNo == idNo && !c.IsDelete);
            if (s != null)
            {
                var payment = db.Payments.Include(c => c.Semester).Include(c => c.Exam).FirstOrDefault(c =>
                        !c.IsDelete && c.ProgramId == s.ProgramId && c.SemesterId == vm.SemesterId &&
                        c.ExamId == vm.ExamId && c.StudentId == s.IdNo);


                var approval = db.AdmitCardApprovals.Any(c => c.PaymentId == payment.Id && !c.IsPrevious && c.IsPaymentComplete && !c.IsDelete && c.StudentInfoId == s.Id);
                var isSpecial = db.AdmitCardApprovals.Any(c => c.PaymentId == payment.Id && !c.IsPrevious && c.IsSpecialPermission && !c.IsDelete && c.StudentInfoId == s.Id);
                var isPending = db.AdmitCardRequests.Any(c => !c.IsDone && c.StudentInfoId == s.Id);

                if (isPending)
                    return Json("00", JsonRequestBehavior.AllowGet);

                var admit = new AdmitCardQuery();
                if (approval)
                {
                    string path = Path.Combine(Server.MapPath("~/Reports"), "AdmitCardReport.rdlc");
                    viewer.LocalReport.ReportPath = path;

                    admit.Id = s.Id;
                    admit.IdNo = s.IdNo;
                    admit.Name = s.Name;
                    admit.Program = s.Program.Name + "(" + s.Program.ShortName + ")";
                    if (payment != null)
                    {
                        admit.Exam = payment.Exam.Name;
                        admit.Semester = payment.Semester.Name + " " + payment.Semester.Year;
                    }
                    else
                    {
                        admit.Exam = "-";
                        admit.Semester = "-";
                    }
                    admit.ContactNo = s.ContactNo;
                    admit.Email = s.Email;
                }

                if (isSpecial)
                {
                    string path = Path.Combine(Server.MapPath("~/Reports"), "DueAdmitCardReport.rdlc");
                    viewer.LocalReport.ReportPath = path;

                    var special = db.AdmitCardApprovals.Where(c =>
                        c.IsSpecialPermission && !c.IsDelete && c.StudentInfoId == s.Id).OrderByDescending(c => c.Id).FirstOrDefault();
                    admit.Id = s.Id;
                    admit.IdNo = s.IdNo;
                    admit.Name = s.Name;
                    admit.Program = s.Program.Name + "(" + s.Program.ShortName + ")";
                    if (payment != null)
                    {
                        admit.Exam = payment.Exam.Name;
                        admit.Semester = payment.Semester.Name + " " + payment.Semester.Year;
                    }
                    else
                    {
                        admit.Exam = "-";
                        admit.Semester = "-";
                    }
                    admit.ContactNo = DateTimeFormatter.DateToString(special.ExceptedDate);
                    admit.Email = s.Email;
                }
                // Image and QR
                if (!string.IsNullOrEmpty(s.ImageFilePath))
                {
                    var image = @"D" + s.ImageFilePath;
                    admit.StudentImage = System.IO.File.ReadAllBytes(image);
                }
                else
                {
                    admit.StudentImage = new byte[0];
                }


                var encoder = new QRCodeEncoder { QRCodeScale = 3 };
                var bmp = encoder.Encode(s.IdNo);
                admit.Qr = ImageToByte(bmp);

                var dataList = new List<AdmitCardQuery>
                                {
                                    admit
                                };


                var rds = new ReportDataSource("DataSet1", dataList);
                viewer.LocalReport.DataSources.Add(rds);


                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension,
                    out streamIds, out warnings);


                string fileName = s.IdNo;
                string outputPath = "~/PdfReports/";
                if (Directory.Exists(Server.MapPath(outputPath)))
                {
                    var files = Directory.GetFiles(Server.MapPath(outputPath));
                    foreach (var f in files)
                    {
                        System.IO.File.Delete(f);
                    }
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath(outputPath));

                }

                using (var stream = System.IO.File.Create(Path.Combine(Server.MapPath(outputPath), fileName + ".pdf")))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                var pdfHref = "/PdfReports/" + fileName + ".pdf";

                return Json(pdfHref, JsonRequestBehavior.AllowGet);

            }

            return Json("0", JsonRequestBehavior.AllowGet);
        }



        // GET: StudentInfoes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentInfo studentInfo = await db.StudentInfos.FindAsync(id);
            if (studentInfo == null)
            {
                return HttpNotFound();
            }
            return View(studentInfo);
        }

        // GET: StudentInfoes/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            var captcha = Captcha.GetCaptcha();
            ViewBag.Base64String = captcha[0];
            ViewBag.Answer = captcha[1];

            ViewBag.Message = "";
            ViewBag.MessageColor = "";
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");


            return View();
        }

        // POST: StudentInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProgramId,IdNo,Name,ContactNo,Email,StudentImageFile, Captcha, WhoIs")] StudentInfoVm vm)
        {

            var isCaptcha = Captcha.CaptchaMatch(vm.WhoIs, vm.Captcha);

            var captcha = Captcha.GetCaptcha();
            ViewBag.Base64String = captcha[0];
            ViewBag.Answer = captcha[1];

            if (isCaptcha)
            {
                if (ModelState.IsValid)
                {
                    var isExist = db.StudentInfos.Any(c => !c.IsDelete && c.IdNo == vm.IdNo);
                    if (!isExist)
                    {
                        var s = new StudentInfo()
                        {
                            ProgramId = vm.ProgramId,
                            IdNo = vm.IdNo,
                            Name = vm.Name,
                            ContactNo = vm.ContactNo,
                            Email = vm.Email
                        };

                        db.StudentInfos.Add(s);
                        var r = await db.SaveChangesAsync();
                        try
                        {
                            var pdfFile1 = vm.StudentImageFile;
                            //var pdfFile2 = vm.StudentPaymentFile;
                            if (pdfFile1 != null)
                            {
                                // Save Path
                                const string drive = "D";

                                var savePathWithoutDrive1 = ":\\NubAdmit\\StudentImage1\\";
                                //var savePathWithoutDrive2 = ":\\NubAdmit\\PaymentImage\\";
                                string fileSavePath1 = drive + savePathWithoutDrive1;
                                //string fileSavePath2 = drive + savePathWithoutDrive2;

                                if (!Directory.Exists(fileSavePath1))
                                {
                                    Directory.CreateDirectory(fileSavePath1);
                                }
                                //if (!Directory.Exists(fileSavePath2))
                                //{
                                //    Directory.CreateDirectory(fileSavePath2);
                                //}

                                var fileName = s.Id + ".jpg";
                                // var fileExtension = Path.GetExtension(pdfFile.FileName);

                                // Check File is Exist
                                if (!System.IO.File.Exists(fileSavePath1 + fileName))
                                {
                                    // Save file
                                    pdfFile1.SaveAs(fileSavePath1 + fileName);

                                    // Save Path in Database
                                    s.ImageFilePath = savePathWithoutDrive1 + fileName;
                                }

                                //if (!System.IO.File.Exists(fileSavePath2 + fileName))
                                //{
                                //    // Save file
                                //    pdfFile2.SaveAs(fileSavePath2 + fileName);

                                //    // Save Path in Database
                                //    s.PaymentFilePath = savePathWithoutDrive2 + fileName;
                                //}
                            }
                        }
                        catch (Exception e)
                        {
                            s.IsDelete = true;
                            await db.SaveChangesAsync();

                            ViewBag.Message = "Error: " + e.Message;
                            ViewBag.MessageColor = "text-danger";
                            return View();
                        }
                        r = await db.SaveChangesAsync();
                        if (r > 0)
                        {
                            ModelState.Clear();
                            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");
                            ViewBag.Message = "Successfully Submitted";
                            ViewBag.MessageColor = "text-success";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);
                        ViewBag.Message = "This ID No. Already in Database";
                        ViewBag.MessageColor = "text-danger";
                        return View();
                    }


                    ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);
                    ViewBag.Message = "Submit Fail";
                    ViewBag.MessageColor = "text-danger";
                    return View(vm);
                }
            }


            ViewBag.Message = "Submit Valid Value and Image";
            ViewBag.MessageColor = "text-warning";
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);
            return View(vm);
        }

        // GET: StudentInfoes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();
            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentInfo s = await db.StudentInfos.FindAsync(id);
            if (s == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", s.ProgramId);

            var vm = new StudentEditVm()
            {
                Id = s.Id,
                IdNo = s.IdNo,
                Name = s.Name,
                ContactNo = s.ContactNo,
                Email = s.Email,
                ProgramId = s.ProgramId
            };

            return View(vm);
        }

        // POST: StudentInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProgramId,SemesterId,ExamId,IdNo,Name,ContactNo,Email")] StudentEditVm vm)
        {
            if (ModelState.IsValid)
            {
                var s = db.StudentInfos.Find(vm.Id);
                if (s != null)
                {
                    s.ProgramId = vm.ProgramId;
                    s.IdNo = vm.IdNo;
                    s.Name = vm.Name;
                    s.ContactNo = vm.ContactNo;
                    s.Email = vm.Email;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("ApprovedList");
            }

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();
            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", vm.ExamId);
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "Name", vm.ProgramId);
            return View(vm);
        }

        // GET: StudentInfoes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentInfo studentInfo = await db.StudentInfos.FindAsync(id);
            if (studentInfo == null)
            {
                return HttpNotFound();
            }
            return View(studentInfo);
        }

        // POST: StudentInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            //StudentInfo studentInfo = await db.StudentInfos.FindAsync(id);
            //db.StudentInfos.Remove(studentInfo);
            //await db.SaveChangesAsync();
            return RedirectToAction("PaymentStatusApproval");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
