using App.Core.Query;
using App.Mvc.Models;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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
                        var studentId = Convert.ToInt64(searchText);


                        var admit = new AdmitCardQuery()
                        {
                            Id = 1,
                            IdNo = "21170100527",
                            Name = "Md. Forhad Parvez",
                            Program = "Bachelor of Science in Computer Science and Engineering (ECSE)",
                            Exam = "Mid",
                            Semester = "Summer 2020",
                            ContactNo = "01761024315",
                            Email = "forhadparvez@outlook.com"
                        };

                        // Image and QR
                        var image = @"E:\PersonImage\1.jpg";
                        admit.StudentImage = File.ReadAllBytes(image);

                        var encoder = new QRCodeEncoder { QRCodeScale = 3 };
                        var bmp = encoder.Encode("21170100527");
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

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}