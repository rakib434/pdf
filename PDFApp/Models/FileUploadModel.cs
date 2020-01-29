using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFApp.Models
{
    public class FileUploadModel
    {
        public string QRCode { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }
}