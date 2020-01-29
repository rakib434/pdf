using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace PDFApp.Models
{
    public static class PDFBuilder
    {
        internal static byte[] InsertImageIntoPDF(byte[] file, Image QrImage, Image FontImage, int QPositionX = 30,int QPositionY=675,int QWidth=100,int QHeight = 100, int FPositionX = 450, int FPositionY = 50, int FWidth = 130, int FHeight = 15 )
        {
            try
            {
                var pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                using (var ms = new MemoryStream())
                {
                    using (var stamp = new iTextSharp.text.pdf.PdfStamper(pdfReader, ms))
                    {
                        var size = pdfReader.GetPageSize(1);
                        var page = pdfReader.NumberOfPages;

                        #region QrCode
                        System.Drawing.Imaging.ImageFormat QrImgformat = FormatImage(QrImage);
                        var pdfQrImage = iTextSharp.text.Image.GetInstance(QrImage, QrImgformat);
                        pdfQrImage.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        pdfQrImage.SetAbsolutePosition(QPositionX, QPositionY);
                        pdfQrImage.ScaleToFit(QWidth, QHeight);
                        stamp.GetOverContent(page).AddImage(pdfQrImage);
                        #endregion

                        #region Font
                        System.Drawing.Imaging.ImageFormat fontImageFormat = FormatImage(FontImage);
                        var pdfFontImage = iTextSharp.text.Image.GetInstance(FontImage, fontImageFormat);
                        pdfFontImage.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                        pdfFontImage.SetAbsolutePosition(FPositionX, FPositionY);
                        pdfFontImage.ScaleToFit(FWidth, FHeight);
                        stamp.GetOverContent(page).AddImage(pdfFontImage);
                        #endregion
                    }
                    ms.Flush();
                    ms.GetBuffer();

                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static System.Drawing.Imaging.ImageFormat FormatImage(Image image)
        {
            System.Drawing.Imaging.ImageFormat format = image.PixelFormat == PixelFormat.Format1bppIndexed
                                               || image.PixelFormat == PixelFormat.Format4bppIndexed
                                               || image.PixelFormat == PixelFormat.Format8bppIndexed
                                                   ? System.Drawing.Imaging.ImageFormat.Tiff
                                                   : System.Drawing.Imaging.ImageFormat.Jpeg;
            return format;
        }
        internal static Image DrawText(String text, System.Drawing.Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            System.Drawing.Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }
        internal static Image GenerateQr(string qr)
        {
            var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chl={0}", qr, 500, 500);
            WebResponse response = default(WebResponse);
            Stream remoteStream = default(Stream);
            StreamReader readStream = default(StreamReader);
            WebRequest request = WebRequest.Create(url);
            response = request.GetResponse();
            remoteStream = response.GetResponseStream();
            readStream = new StreamReader(remoteStream);
            Image img = Image.FromStream(remoteStream);

            response.Close();
            remoteStream.Close();
            readStream.Close();
            byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));
            string imgString = Convert.ToBase64String(bytes);
            return img;
        }
        internal static Image GenerateOfflineQr(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText,
            QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return qrCodeImage;
        }
    }
}