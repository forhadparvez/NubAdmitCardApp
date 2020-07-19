using App.Core.Query;
using App.Mvc.Models;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;

namespace App.Mvc.Reports
{
    public partial class SectorReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string searchText = string.Empty;
                string sector = string.Empty;

                if (Request.QueryString["searchText"] != null)
                {
                    searchText = Request.QueryString["searchText"].ToString();
                }

                using (var context = new ApplicationDbContext())
                {
                    if (searchText != "")
                    {
                        var db = new ApplicationDbContext();

                        var idNo = searchText;

                        var s = db.StudentInfos.Include(c => c.Exam).Include(c => c.Program).Include(c => c.Semester)
                            .SingleOrDefault(c => c.IdNo == idNo);
                        if (s != null)
                        {
                            var approval = db.AdmitCardApprovals.Any(c => c.IsPaymentComplete && !c.IsDelete);

                            if (approval)
                            {
                                var admit = new AdmitCardQuery()
                                {
                                    Id = s.Id,
                                    IdNo = s.IdNo,
                                    Name = s.Name,
                                    Program = s.Program.Name + "(" + s.Program.ShortName + ")",
                                    Exam = s.Exam.Name,
                                    Semester = s.Semester.Name + " " + s.Semester.Year,
                                    ContactNo = s.ContactNo,
                                    Email = s.Email
                                };

                                // Image and QR
                                var image = @"D" + s.ImageFilePath;
                                admit.StudentImage = File.ReadAllBytes(image);

                                var encoder = new QRCodeEncoder { QRCodeScale = 3 };
                                var bmp = encoder.Encode(s.IdNo);
                                admit.Qr = ImageToByte(bmp);

                                var dataList = new List<AdmitCardQuery>
                                {
                                    admit
                                };


                                ReportViewer1.LocalReport.ReportPath = Server.MapPath("/Reports/AdmitCardReport.rdlc");
                                ReportViewer1.LocalReport.DataSources.Clear();

                                var rdc = new ReportDataSource("DataSet1", dataList);
                                ReportViewer1.LocalReport.DataSources.Add(rdc);

                                ReportViewer1.LocalReport.Refresh();
                                ReportViewer1.DataBind();
                            }
                        }
                    }
                }
            }
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}