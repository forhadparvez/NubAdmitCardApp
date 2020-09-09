using App.Core.Application;
using App.Core.Entities;
using App.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Mvc.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult PaymentList()
        {
            var entity = db.Payments.Where(c => !c.IsDelete)
                .Include(c => c.Exam)
                .Include(c => c.Program)
                .Include(c => c.Semester)
                .OrderBy(c => c.StudentId);
            return View(entity);
        }


        public ActionResult DuePaymentAutoApprove()
        {
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            return View();
        }

        [HttpPost]
        public ActionResult DuePaymentAutoApprove(PaymentUploadVm vm)
        {
            var user = User.Identity.Name;

            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            var entity = db.Payments.Where(c => !c.IsDelete && c.SemesterId == vm.SemesterId && c.ExamId == vm.ExamId && c.DuesPercentAmount <= vm.MaxDuePayment && c.DuesPercentAmount >= vm.MinDuePayment).ToList();
            foreach (var p in entity)
            {
                var student = db.StudentInfos.FirstOrDefault(c => c.IdNo == p.StudentId && !c.IsDelete);

                if (student != null)
                {
                    if (db.AdmitCardApprovals.Any(c => c.PaymentId == p.Id && c.IsPaymentComplete))
                        continue;

                    var li = db.AdmitCardApprovals.Where(c => c.StudentInfoId == student.Id);
                    foreach (var ap in li)
                    {
                        ap.IsPrevious = true;
                    }
                    db.SaveChanges();


                    var stds = db.AdmitCardRequests.Where(c => c.StudentInfoId == student.Id && !c.IsDone);
                    foreach (var std in stds)
                    {
                        std.IsDone = true;
                        std.Status = true;
                    }
                    db.SaveChanges();

                    var a = new AdmitCardApproval()
                    {
                        PaymentId = p.Id,
                        StudentInfoId = student.Id,
                        ExceptedDate = DateTimeFormatter.StringToDate(vm.PermissionDate),
                        IsSpecialPermission = true,
                        Comments = "",
                        IsPrevious = false,
                        ApproveBy = user,
                        ApproveDate = DateTime.Now
                    };

                    db.AdmitCardApprovals.Add(a);

                    var r = db.SaveChanges();
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PaymentAutoApprove()
        {
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            return View();
        }

        [HttpPost]
        public ActionResult PaymentAutoApprove(PaymentUploadVm vm)
        {
            var user = User.Identity.Name;

            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            var entity = db.Payments.Where(c => !c.IsDelete && c.SemesterId == vm.SemesterId && c.ExamId == vm.ExamId && c.DuesPercentAmount <= 100).ToList();
            foreach (var p in entity)
            {
                var student = db.StudentInfos.FirstOrDefault(c => c.IdNo == p.StudentId && !c.IsDelete);

                if (student != null)
                {
                    if (db.AdmitCardApprovals.Any(c => c.PaymentId == p.Id && c.IsPaymentComplete))
                        continue;

                    var li = db.AdmitCardApprovals.Where(c => c.StudentInfoId == student.Id);
                    foreach (var ap in li)
                    {
                        ap.IsPrevious = true;
                    }
                    db.SaveChanges();

                    var a = new AdmitCardApproval()
                    {
                        PaymentId = p.Id,
                        StudentInfoId = student.Id,
                        IsPaymentComplete = false,
                        ApproveBy = user,
                        ApproveDate = DateTime.Now
                    };

                    db.AdmitCardApprovals.Add(a);

                    var r = db.SaveChanges();
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        // GET: PaymentUpload
        public ActionResult Upload()
        {
            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name");
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName");

            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();

            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            return View();
        }


        [HttpPost]
        public ActionResult Upload(PaymentUploadVm vm)
        {
            var semester = db.Semesters.SingleOrDefault(c => c.IsActive);
            if (semester == null) return View();
            ViewBag.SemesterId = semester.Id;
            ViewBag.Semester = semester.Name + " " + semester.Year;

            ViewBag.ExamId = new SelectList(db.Exams, "Id", "Name", vm.ExamId);
            ViewBag.ProgramId = new SelectList(db.Programs, "Id", "ShortName", vm.ProgramId);

            try
            {
                if (vm.File != null)
                {
                    OleDbConnection excelConnection = null;
                    var path = "";

                    try
                    {
                        var root = Server.MapPath("~/ImportedExcel/");
                        if (!Directory.Exists(root))
                        {
                            Directory.CreateDirectory(root);
                        }

                        vm.File.SaveAs(root + Path.GetFileName(vm.File.FileName));

                        path = $"{Server.MapPath("~/ImportedExcel")}/{vm.File.FileName}";


                        switch (Path.GetExtension(path))
                        {
                            case ".xls":
                                excelConnection =
                                    new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path +
                                                        ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"");
                                excelConnection.Open();
                                break;
                            case ".xlsx":
                                excelConnection =
                                    new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path +
                                                        ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
                                excelConnection.Open();
                                break;
                        }

                        var cmd = new OleDbCommand
                        {
                            Connection = excelConnection,
                            CommandType = CommandType.Text,
                            CommandText = "Select * from [Sheet1$]"
                        };

                        var result = new List<Payment>();

                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    var studentId = reader[1].ToString();

                                    var entityInDb = db.Payments.SingleOrDefault(c =>
                                        c.SemesterId == vm.SemesterId && c.ExamId == vm.ExamId &&
                                        c.StudentId == studentId && !c.IsDelete);
                                    if (entityInDb == null)
                                    {
                                        var p = new Payment()
                                        {
                                            ProgramId = vm.ProgramId,
                                            SemesterId = vm.SemesterId,
                                            ExamId = vm.ExamId,

                                            StudentId = studentId,
                                            StudentName = reader[2].ToString(),
                                            WaiverPercent = StringToNumber(reader[3].ToString()),
                                            CreditTaken = StringToNumber(reader[4].ToString()),
                                            NetPayable = StringToNumber(reader[5].ToString()),
                                            PreviousDues = StringToNumber(reader[6].ToString()),
                                            TotalPayable = StringToNumber(reader[7].ToString()),
                                            PayablePercentAmount = StringToNumber(reader[8].ToString()),
                                            ReceivedAmount = StringToNumber(reader[9].ToString()),
                                            DuesPercentAmount = StringToNumber(reader[10].ToString()),
                                            TotalDues = StringToNumber(reader[11].ToString())
                                        };

                                        //if (!db.StudentInfos.Any(c => c.IdNo == studentId))
                                        //{
                                        //    var std = new StudentInfo()
                                        //    {
                                        //        ProgramId = vm.ProgramId,
                                        //        IdNo = studentId,
                                        //        Name = p.StudentName,
                                        //        ContactNo = "-",
                                        //        Email = "demo@mail.com"
                                        //    };

                                        //    db.StudentInfos.Add(std);
                                        //    db.SaveChanges();
                                        //}


                                        result.Add(p);
                                    }
                                    else
                                    {
                                        entityInDb.WaiverPercent = StringToNumber(reader[3].ToString());
                                        entityInDb.CreditTaken = StringToNumber(reader[4].ToString());
                                        entityInDb.NetPayable = StringToNumber(reader[5].ToString());
                                        entityInDb.PreviousDues = StringToNumber(reader[6].ToString());
                                        entityInDb.TotalPayable = StringToNumber(reader[7].ToString());
                                        entityInDb.PayablePercentAmount = StringToNumber(reader[8].ToString());
                                        entityInDb.ReceivedAmount = StringToNumber(reader[9].ToString());
                                        entityInDb.DuesPercentAmount = StringToNumber(reader[10].ToString());
                                        entityInDb.TotalDues = StringToNumber(reader[11].ToString());

                                        db.SaveChanges();
                                    }
                                }
                                catch (Exception e)
                                {
                                    //
                                }
                            }

                            db.Payments.AddRange(result);

                            db.SaveChanges();
                        }

                        reader.Close();



                        return Json(1, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {

                        return Json(ex.Message, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        OleDbConnection.ReleaseObjectPool();
                        if (excelConnection != null)
                        {
                            excelConnection.Close();
                            excelConnection.Dispose();
                        }

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                }


                return Json("No Data", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        private decimal StringToNumber(string number)
        {
            try
            {
                var value = number.Split(' ');
                var n = value[0].Replace(",", "");
                return Convert.ToDecimal(n);
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}