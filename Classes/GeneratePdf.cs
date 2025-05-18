using System;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MF_Shopping_Assistant.Classes
{
    internal class GeneratePdf
    {
        public void GeneratePdfFile(string pdfName, string toEmail)
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "pdfs");
                Directory.CreateDirectory(folderPath);
                string pdfPath = Path.Combine(folderPath, pdfName);

                using (FileStream stream = new FileStream(pdfPath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Title
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.BLUE);
                    Paragraph title = new Paragraph("Invoice", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    document.Add(title);

                    document.Add(new Paragraph($"Date: {DateTime.Now.ToShortDateString()}"));

                    document.Add(new Paragraph(" "));

                    // From / To Info
                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.AddCell("From:\nFodyConfig\nEmail: fody.cfg@gmail.com");
                    infoTable.AddCell($"To:\nEmail: {toEmail}");
                    document.Add(infoTable);

                    document.Add(new Paragraph("\n"));

                    // Table
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    float[] widths = new float[] { 10, 30, 20, 20, 20 };
                    table.SetWidths(widths);

                    // Header
                    AddCell(table, "#", true);
                    AddCell(table, "Product", true);
                    AddCell(table, "Unit price", true);
                    AddCell(table, "Quantity", true);
                    AddCell(table, "Total", true);

                    double grandTotal = 0;

                    for (int i = 0; i < GlobalData.listIdsOfProducts.Count; i++)
                    {
                        string name = GlobalData.listNameOfProducts[i];
                        double price = GlobalData.listPricePerUnitOfProducts[i];
                        double quantity = GlobalData.listQuantityOfProducts[i];
                        double temp = quantity;
                        if (GlobalData.panelsOfProducts[i].Name == "Fruit") quantity /= 1000;
                        double total = (price * quantity);
                        grandTotal += total;

                        AddCell(table, (i + 1).ToString());
                        AddCell(table, name);
                        AddCell(table, price.ToString("N2") + " KM");
                        AddCell(table, temp.ToString("N2"));
                        AddCell(table, total.ToString("N2") + " KM");
                    }

                    document.Add(table);

                    document.Add(new Paragraph($"\nTotal price: {grandTotal.ToString("N2")} KM"));

                    document.Close();
                        
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF generation error: " + ex.Message);
            }
        }

        private void AddCell(PdfPTable table, string text, bool isHeader = false)
        {
            var font = isHeader
                ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)
                : FontFactory.GetFont(FontFactory.HELVETICA, 11);

            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5,
                BackgroundColor = isHeader ? new BaseColor(220, 220, 220) : BaseColor.WHITE
            };
            table.AddCell(cell);
        }
    }
}
