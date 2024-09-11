using iTextSharp.text.pdf;

namespace UploadBobsWorkers.Common
{
    public static class Compressor
    {
        public static string CompressPdfBase64(string base64Input)
        {
            byte[] pdfBytes = Convert.FromBase64String(base64Input);

            using MemoryStream inputPdfStream = new(pdfBytes);
            using MemoryStream outputPdfStream = new();
            
            PdfReader reader = new (inputPdfStream);
            PdfStamper stamper = new (reader, outputPdfStream, PdfWriter.VERSION_1_7);
            stamper.Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

            int total = reader.NumberOfPages + 1;
            for (int i = 1; i < total; i++)
            {
                PdfDictionary page = reader.GetPageN(i);
                PdfObject pdfObject = page.Get(PdfName.CONTENTS);

                if (pdfObject.IsIndirect())
                {
                    pdfObject = PdfReader.GetPdfObject(pdfObject);
                }

                if (pdfObject.IsArray())
                {
                    PdfArray array = (PdfArray)pdfObject;
                    for (int j = 0; j < array.Size; j++)
                    {
                        PdfObject element = array.GetPdfObject(j);
                        if (element.IsStream())
                        {
                            CompressPdfStream((PRStream)element, stamper);
                        }
                    }
                }
                else if (pdfObject.IsStream())
                {
                    CompressPdfStream((PRStream)pdfObject, stamper);
                }
            }

            stamper.Close();
            reader.Close();

            byte[] compressedPdfBytes = outputPdfStream.ToArray();
            return Convert.ToBase64String(compressedPdfBytes);
        }

        private static void CompressPdfStream(PRStream stream, PdfStamper stamper)
        {
            PdfName filter = stream.GetAsName(PdfName.FILTER);
            if(filter != null && (filter.Equals(PdfName.DCTDECODE) || filter.Equals(PdfName.JPXDECODE)))
            {
                try
                {
                    byte[] streamBytes = PdfReader.GetStreamBytes(stream);
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(streamBytes);

                    if (img != null)
                    {
                        img.ScalePercent(40);
                        PdfContentByte content = stamper.GetOverContent(1);
                        content.AddImage(img);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No es una imagen o falló la compresión: " + ex.Message);
                }
            }
        }
    }
}
