using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace GameStore.Web;

public class InvoiceGenerator
{
    public static byte[] GenerateInvoice(Guid customerId, Guid orderId, double sum)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(16));

                page.Header()
                    .AlignCenter()
                    .Text($"Invoice for Order: {orderId}")
                    .SemiBold().FontSize(24).FontColor(Colors.Grey.Darken4);

                page.Content()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(20);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("#");
                                    header.Cell().Text("Detail");
                                    header.Cell().AlignRight().Text("Value");
                                });

                                table.Cell().Text("1");
                                table.Cell().Text("User ID");
                                table.Cell().Text(customerId.ToString());

                                table.Cell().Text("2");
                                table.Cell().Text("Order ID");
                                table.Cell().Text(orderId.ToString());

                                table.Cell().Text("3");
                                table.Cell().Text("Creation Date");
                                table.Cell().Text(DateTime.Now.ToString());

                                table.Cell().Text("4");
                                table.Cell().Text("Sum");
                                table.Cell().Text(sum.ToString());
                            });
            });
        })
    .GeneratePdf();
    }
}
