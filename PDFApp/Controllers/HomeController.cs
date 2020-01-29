using PDFApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PDFApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Index(FileUploadModel fileUpload)
        {
            MemoryStream target = new MemoryStream();
            fileUpload.ImageFile.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            string fileType = Path.GetExtension(fileUpload.ImageFile.FileName);
            string fileName = fileUpload.ImageFile.FileName;
            byte[] filePdf = data;
            string QrCode = fileUpload.QRCode;

           // Image QrImage = PDFBuilder.GenerateQr(QrCode);
            Image QrImage = PDFBuilder.GenerateOfflineQr(QrCode);
            
            Font font = new Font("Times New Roman", 30.0f);
            string hexValue = "#000000";
            Color color = System.Drawing.ColorTranslator.FromHtml(hexValue);
            string hexValueBack = "#FFFFFF";
            Color backColor = System.Drawing.ColorTranslator.FromHtml(hexValueBack);
            string Footer = "Date: " + DateTime.Now.ToShortDateString();
            
            Image FontImage = PDFBuilder.DrawText(Footer, font, color, backColor);
            byte[] pdf = PDFBuilder.InsertImageIntoPDF(filePdf, QrImage, FontImage);
            DownLoadPdf(pdf, fileName, fileType);
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
   
        private void DownLoadPdf(byte[] pdfFile, string fileName, string fileType)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = fileType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(pdfFile);
            Response.Flush();
            Response.End();
        }
    }
}