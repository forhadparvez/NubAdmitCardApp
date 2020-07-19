using App.Core.Entities;
using App.Mvc.Models;
using App.Mvc.ViewModels;
using System;
using System.Data.Entity;
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
        public async Task<ActionResult> Index()
        {
            var studentInfos = db.StudentInfos.Include(s => s.Exam).Include(s => s.Program).Include(s => s.Semester).Where(c => !c.IsDelete);
            return View(await studentInfos.ToListAsync());
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
            StudentInfo studentInfo = await db.StudentInfos.FindAsync(id);
            db.StudentInfos.Remove(studentInfo);
            await db.SaveChangesAsync();
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
