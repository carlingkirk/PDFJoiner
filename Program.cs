using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PDFJoiner
{
    class Program
    {
        static void Main(string[] args)
        {
            var filepath = new DirectoryInfo(args[0]);
            var pdfFiles = filepath.EnumerateFiles("*.pdf").OrderBy(f => f.CreationTimeUtc).ToList();
            if (pdfFiles.Count < 2)
            {
                throw new InvalidOperationException($"Only found {pdfFiles.Count} files!");
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var pdfResult = new PdfDocument();
            var font = new XFont("Consolas", 10, XFontStyle.Bold);
            var format = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Far
            };

            foreach (var file in pdfFiles)
            {
                using (var pdfFile = PdfReader.Open(file.FullName, PdfDocumentOpenMode.Import))
                {
                    pdfResult.Pages.Add(pdfFile.Pages[0]);

                    var page = pdfResult.Pages[pdfResult.Pages.Count - 1];
                    // Write document file name
                    var gfx = XGraphics.FromPdfPage(page);
                    var box = page.MediaBox.ToXRect();
                    box.Inflate(0, -10);
                    gfx.DrawString($"{file.Name}", font, XBrushes.Black, box, format);
                }
            }

            var filename = pdfFiles.First().Name.Replace(".pdf", "") + "-" + pdfFiles.Last().Name;
            pdfResult.Save(filepath + "/" + filename);
        }
    }
}
