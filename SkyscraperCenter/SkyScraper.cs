using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SkyscraperCenter
{
    class SkyScraper
    {
        const string pdfFileName = "Building List 6-10-2019.pdf";
        static readonly List<string> ignoreUrls = new List<string>
        {
            "http://skyscrapercenter.com","http://www.ctbuh.org"
        };

        public void GetData()
        {
            var urls = GetAllLinksFromPdf();
            SaveHtmlFiles(urls);
        }

        public List<string> GetAllLinksFromPdf()
        {
            // https://stackoverflow.com/questions/8140339/using-itextsharp-to-extract-and-update-links-in-an-existing-pdf

            var urls = new List<string>();
            var r = new PdfReader(pdfFileName);
            var index = 0;

            for (int i = 1; i <= r.NumberOfPages; i++)
            {
                //Get the current page
                var PageDictionary = r.GetPageN(i);

                //Get all of the annotations for the current page
                var Annots = PageDictionary.GetAsArray(PdfName.ANNOTS);

                //Make sure we have something
                if ((Annots == null) || (Annots.Length == 0))
                    continue;
                foreach (var A in Annots.ArrayList)
                {
                    var AnnotationDictionary = PdfReader.GetPdfObject(A) as PdfDictionary;
                    if (AnnotationDictionary == null)
                        continue;
                    //Make sure this annotation has a link
                    if (!AnnotationDictionary.Get(PdfName.SUBTYPE).Equals(PdfName.LINK))
                        continue;

                    //Make sure this annotation has an ACTION
                    if (AnnotationDictionary.Get(PdfName.A) == null)
                        continue;

                    var annotActionObject = AnnotationDictionary.Get(PdfName.A);
                    var AnnotationAction = (PdfDictionary)(annotActionObject.IsIndirect() ? PdfReader.GetPdfObject(annotActionObject) : annotActionObject);

                    var type = AnnotationAction.Get(PdfName.S);
                    //Test if it is a URI action
                    if (type.Equals(PdfName.URI))
                    {
                        //Change the URI to something else
                        var relativeRef = AnnotationAction.GetAsString(PdfName.URI).ToString();
                        urls.Add(relativeRef);
                        Console.WriteLine($"Added Url :- {++index}");
                    }
                }
            }
            r.Close();

            return urls;
        }

        public void SaveHtmlFiles(List<string> urls)
        {
            try
            {
                var index = 0;
                if (urls?.Count > 0)
                {
                    foreach (var url in urls)
                    {
                        if (ignoreUrls.Contains(url) || !url.Contains("building"))
                            continue;

                        Thread.Sleep(2000);
                        var html = HelperClass.GetHtmlFromUrl(url);
                        var splitUrl = url.Split('/');
                        var fileName = $"{++index}_{splitUrl[splitUrl.Length - 2]}.html";
                        var path = $"C:\\Local Project\\SkyscraperCenter\\SkyscraperCenter\\HtmlFiles\\{fileName}";
                        File.WriteAllText(path, html);
                        Console.Write($"Saved File {fileName}");
                    }
                }
                Console.WriteLine("Finish.");
            }
            catch (Exception e)
            {

            }

        }

        public void ParsePdf()
        {
            // http://www.squarepdf.net/parsing-pdf-files-using-itextsharp
            var pdfPages = new List<string>();
            using (var reader = new PdfReader(pdfFileName))
            {
                // Read pages
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                    pdfPages.Add(pageText);
                }
            }

        }
    }
}
