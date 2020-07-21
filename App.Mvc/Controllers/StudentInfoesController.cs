using App.Core.Application;
using App.Core.Command;
using App.Core.Entities;
using App.Core.Query;
using App.Mvc.Models;
using App.Mvc.ViewModels;
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

        // GET: StudentInfoes
        public async Task<ActionResult> ApprovedList()
        {
            var stdresult = new List<StudentInfo>();

            var studentInfos = await db.StudentInfos.Include(s => s.Exam).Include(s => s.Program).Include(s => s.Semester).Where(c => !c.IsDelete).ToListAsync();
            foreach (var std in studentInfos)
            {
                var isExist = db.AdmitCardApprovals.Any(c => c.StudentInfoId == std.Id && !c.IsDelete);
                if (isExist)
                    stdresult.Add(std);
            }

            return View(stdresult);
        }

        public async Task<ActionResult> PaymentStatusApproval()
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

            var stdresult = new List<StudentInfo>();

            var studentInfos = await db.StudentInfos.Include(s => s.Exam).Include(s => s.Program).Include(s => s.Semester).Where(c => !c.IsDelete).ToListAsync();
            foreach (var std in studentInfos)
            {
                var isExist = db.AdmitCardApprovals.Any(c => c.StudentInfoId == std.Id && !c.IsDelete);
                if (!isExist)
                    stdresult.Add(std);
            }


            return View(stdresult);
        }

        [HttpGet]
        public async Task<JsonResult> GetDetails(long stdId)
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



            var std = await db.StudentInfos.Include(s => s.Exam).Include(s => s.Program).Include(s => s.Semester).SingleOrDefaultAsync(c => !c.IsDelete && c.Id == stdId);

            if (std != null)
            {
                var result = new PaymentStatusApprovalQuery()
                {
                    Id = std.Id,
                    IdNo = std.IdNo,
                    Name = std.Name
                };


                var request = await db.AdmitCardRequests.SingleOrDefaultAsync(c => !c.IsDone && c.StudentInfoId == std.Id);
                if (request != null)
                {
                    result.RequestId = request.Id.ToString();
                    result.RequestedDate = DateTimeFormatter.DateToString(request.RequestedDate);
                    request.Comment = request.Comment;
                }

                var link = "D" + std.PaymentFilePath;
                var sourchFile = Path.Combine(link);
                var tergetPath = Path.Combine(root + @"\StudentInfoes", std.Id + ".jpg");

                System.IO.File.Copy(sourchFile, tergetPath, true);
                result.PaymentFilePath = std.Id + ".jpg";


                var request2 = await db.AdmitCardRequests.Where(c => c.IsDone && c.Status).ToListAsync();
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

            var s = db.StudentInfos.Include(c => c.Exam).Include(c => c.Program).Include(c => c.Semester)
                .SingleOrDefault(c => c.IdNo == idNo);
            if (s != null)
            {
                var approval = db.AdmitCardApprovals.Any(c => c.IsPaymentComplete && !c.IsDelete && !c.IsPrevious && c.StudentInfoId == s.Id);
                var isSpecial = db.AdmitCardApprovals.Any(c => c.IsSpecialPermission && !c.IsDelete && !c.IsPrevious && c.StudentInfoId == s.Id);

                var admit = new AdmitCardQuery();
                if (approval)
                {
                    string path = Path.Combine(Server.MapPath("~/Reports"), "AdmitCardReport.rdlc");
                    viewer.LocalReport.ReportPath = path;

                    admit.Id = s.Id;
                    admit.IdNo = s.IdNo;
                    admit.Name = s.Name;
                    admit.Program = s.Program.Name + "(" + s.Program.ShortName + ")";
                    admit.Exam = s.Exam.Name;
                    admit.Semester = s.Semester.Name + " " + s.Semester.Year;
                    admit.ContactNo = s.ContactNo;
                    admit.Email = s.Email;
                }

                if (isSpecial)
                {
                    string path = Path.Combine(Server.MapPath("~/Reports"), "DueAdmitCardReport.rdlc");
                    viewer.LocalReport.ReportPath = path;

                    var special = db.AdmitCardApprovals.SingleOrDefault(c =>
                        c.IsSpecialPermission && !c.IsDelete && !c.IsPrevious && c.StudentInfoId == s.Id);
                    admit.Id = s.Id;
                    admit.IdNo = s.IdNo;
                    admit.Name = s.Name;
                    admit.Program = s.Program.Name + "(" + s.Program.ShortName + ")";
                    admit.Exam = s.Exam.Name;
                    admit.Semester = s.Semester.Name + " " + s.Semester.Year;
                    admit.ContactNo = DateTimeFormatter.DateToString(special.ExceptedDate);
                    admit.Email = s.Email;
                }
                // Image and QR
                var image = @"D" + s.ImageFilePath;
                admit.StudentImage = System.IO.File.ReadAllBytes(image);

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

            return Json(0, JsonRequestBehavior.AllowGet);
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
            ViewBag.Message = "";
            ViewBag.MessageColor = "";
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            return View();
        }

        // POST: StudentInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProgramId,ExamId,SemesterId,IdNo,Name,ContactNo,Email,StudentImageFile,StudentPaymentFile")] StudentInfoVm vm)
        {
            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();
            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            if (ModelState.IsValid)
            {
                var isExist = db.StudentInfos.Any(c => !c.IsDelete && c.IdNo == vm.IdNo);
                if (!isExist)
                {
                    var s = new StudentInfo()
                    {
                        ProgramId = vm.ProgramId,
                        ExamId = vm.ExamId,
                        SemesterId = vm.SemesterId,
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
                        var pdfFile2 = vm.StudentPaymentFile;
                        if (pdfFile1 != null && pdfFile2 != null)
                        {
                            // Save Path
                            const string drive = "D";

                            var savePathWithoutDrive1 = ":\\NubAdmit\\StudentImage\\";
                            var savePathWithoutDrive2 = ":\\NubAdmit\\PaymentImage\\";
                            string fileSavePath1 = drive + savePathWithoutDrive1;
                            string fileSavePath2 = drive + savePathWithoutDrive2;

                            if (!Directory.Exists(fileSavePath1))
                            {
                                Directory.CreateDirectory(fileSavePath1);
                            }
                            if (!Directory.Exists(fileSavePath2))
                            {
                                Directory.CreateDirectory(fileSavePath2);
                            }

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

                            if (!System.IO.File.Exists(fileSavePath2 + fileName))
                            {
                                // Save file
                                pdfFile2.SaveAs(fileSavePath2 + fileName);

                                // Save Path in Database
                                s.PaymentFilePath = savePathWithoutDrive2 + fileName;
                            }
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
                        ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
                        ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");
                        ViewBag.Message = "Successfully Submitted";
                        ViewBag.MessageColor = "text-success";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", vm.ExamId);
                    ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);
                    ViewBag.Message = "This ID No. Already in Database";
                    ViewBag.MessageColor = "text-danger";
                    return View();
                }


                ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", vm.ExamId);
                ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);
                ViewBag.Message = "Submit Fail";
                ViewBag.MessageColor = "text-danger";
                return View(vm);
            }

            ViewBag.Message = "Submit Valid Value and Image";
            ViewBag.MessageColor = "text-warning";
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", vm.ExamId);
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);
            return View(vm);
        }

        // GET: StudentInfoes/Edit/5
        public async Task<ActionResult> Edit(long? id)
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
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", studentInfo.ExamId);
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "Name", studentInfo.ProgramId);
            ViewBag.SemesterId = new SelectList(db.Semesters, "Id", "Name", studentInfo.SemesterId);
            return View(studentInfo);
        }

        // POST: StudentInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProgramId,SemesterId,ExamId,IdNo,Name,ContactNo,Email,ImageFilePath,PaymentFilePath,IsDelete,EditBy,EditDate,DeleteBy,DeleteDate")] StudentInfo studentInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", studentInfo.ExamId);
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "Name", studentInfo.ProgramId);
            ViewBag.SemesterId = new SelectList(db.Semesters, "Id", "Name", studentInfo.SemesterId);
            return View(studentInfo);
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
            return RedirectToAction("Index");
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
