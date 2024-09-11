using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;

namespace UploadBobsWorkers.Common
{
    public class ImageRenderListener(PdfPage newPage, int aux) : IEventListener
    {
        private readonly PdfPage _newPage = newPage;
        private int _aux = aux;

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_IMAGE)
            {
                _aux = 2;
                ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                PdfImageXObject imageObject = renderInfo.GetImage();
                if (imageObject != null)
                {
                    byte[] originalImageBytes = imageObject.GetImageBytes();
                    byte[] compressedImageBytes = CompressImage(originalImageBytes);

                    AddCompressedImageToNewPage(renderInfo, compressedImageBytes);
                }
            }
        }

        // Método para comprimir la imagen usando ImageSharp, reduciendo su calidad
        private static byte[] CompressImage(byte[] imageBytes)
        {
            using MemoryStream inputStream = new(imageBytes);
            using MemoryStream outputStream = new();
            // Cargar la imagen en ImageSharp
            using (Image image = Image.Load(inputStream))
            {
                // Reducir la calidad de la imagen y guardarla como JPEG
                image.Save(outputStream, new JpegEncoder { Quality = 40 }); // 40% de calidad
            }

            return outputStream.ToArray();
        }

        // Añade la imagen comprimida al nuevo PDF
        private void AddCompressedImageToNewPage(ImageRenderInfo renderInfo, byte[] compressedImageBytes)
        {
            PdfImageXObject compressedImageObject = new(ImageDataFactory.Create(compressedImageBytes));
            // Crear un PdfCanvas para la página y aplicar la matriz de transformación
            PdfCanvas pdfCanvas = new (_newPage.NewContentStreamAfter(), _newPage.GetResources(), _newPage.GetDocument());
            // Añadir la imagen comprimida en la nueva página
            pdfCanvas.AddXObjectAt(compressedImageObject, 0, 0);
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return [EventType.RENDER_IMAGE];
        }
    }
}
