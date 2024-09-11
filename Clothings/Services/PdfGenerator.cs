using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace Clothings.Services
{
    public class PdfGenerator
    {
        public byte[] GeneratePdfFromHtml(string customerName, string email, string shippingAddress, List<userCart> cartItems, decimal totalAmount)
        {
            using var stream = new MemoryStream();

            // Define the PDF document
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header()
                        .Text("Invoice")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(5); // Add spacing between each item

                            column.Item().Text($"Customer Name: {customerName}");
                            column.Item().Text($"Email: {email}");
                            column.Item().Text($"Shipping Address: {shippingAddress}");
                            column.Item().Text("Order Items:").Bold();

                            column.Item().Table(table =>
                            {
                                // Define table columns
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Product column
                                    columns.RelativeColumn(); // Quantity column
                                    columns.RelativeColumn(); // Price column
                                    columns.RelativeColumn(); // Total column
                                });

                                // Add table header
                                table.Header(header =>
                                {
                                    header.Cell().Text("Product").Bold();
                                    header.Cell().Text("Quantity").Bold();
                                    header.Cell().Text("Unit Price").Bold();
                                    header.Cell().Text("Total").Bold();
                                });

                                // Add table rows for each cart item
                                foreach (var item in cartItems)
                                {
                                    table.Cell().Text(item.product.Name);
                                    table.Cell().Text(item.Quantity.ToString());
                                    table.Cell().Text($"{item.product.Price:C}");
                                    table.Cell().Text($"{(item.Quantity * item.product.Price):C}");
                                }
                            });

                            column.Item().Text($"Total Amount: {totalAmount:C}")
                                .Bold()
                                .FontSize(14)
                                .AlignRight();
                        });
                });
            }).GeneratePdf(stream);

            return stream.ToArray(); // Return the PDF as a byte array
        }
    }
}
