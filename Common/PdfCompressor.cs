using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;

namespace UploadBobsWorkers.Common
{
    public static class PdfCompressor
    {
        public static string CompressPdfBase64(string base64Input)
        {
            byte[] pdfBytes = Convert.FromBase64String(base64Input);

            using MemoryStream inputPdfStream = new (pdfBytes);
            using MemoryStream outputPdfStream = new ();

            PdfReader reader = new (inputPdfStream);
            PdfWriter writer = new (outputPdfStream, new WriterProperties().SetFullCompressionMode(true).SetCompressionLevel(9));
            PdfDocument originalPdf = new (reader);
            PdfDocument newPdf = new (writer);

            // Procesar cada página y reducir la calidad de las imágenes
            int total = originalPdf.GetNumberOfPages();
            for (int i = 1; i <= total; i++)
            {
                PdfPage originalPage = originalPdf.GetPage(i);
                PdfPage newPage = newPdf.AddNewPage(new PageSize(originalPage.GetPageSize()));

                // Now process and add the images
                AddCompressedImages(originalPage, newPage);
            }

            originalPdf.Close();
            newPdf.Close();

            byte[] compressedPdfBytes = outputPdfStream.ToArray();
            return Convert.ToBase64String(compressedPdfBytes);
        }

        private static void AddCompressedImages(PdfPage originalPage, PdfPage newPage)
        {
            PdfCanvasProcessor processor = new (new ImageRenderListener(newPage, 1));
            processor.ProcessPageContent(originalPage);
        }
    }
}
