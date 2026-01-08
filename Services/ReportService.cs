using expensemanager.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace expensemanager.Services
{
    /// <summary>
    /// Implements PDF reporting using QuestPDF, enforcing per-user isolation.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportService"/>.
        /// </summary>
        public ReportService(ApplicationDbContext db) => _db = db;

        /// <inheritdoc />
        public async Task<byte[]> GenerateUserExpenseSummaryAsync(int userId, CancellationToken ct = default)
        {
            var items = await _db.Expenses.AsNoTracking()
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync(ct);

            var total = items.Sum(i => i.Amount);
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var monthly = items.Where(i => i.Date >= monthStart).Sum(i => i.Amount);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeColumn().Stack(stack =>
                            {
                                stack.Item().Text("Expense Summary").SemiBold().FontSize(20);
                                stack.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(10).Light();
                            });
                            row.ConstantColumn(100).Stack(stack =>
                            {
                                stack.Item().Text($"Total: {total:C}").SemiBold();
                                stack.Item().Text($"This Month: {monthly:C}");
                            });
                        });

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Title").SemiBold();
                                header.Cell().Text("Amount").SemiBold();
                                header.Cell().Text("Date").SemiBold();
                                header.Cell().Text("Category").SemiBold();
                            });

                            foreach (var i in items)
                            {
                                table.Cell().Text(i.Title);
                                table.Cell().Text(i.Amount.ToString("C"));
                                table.Cell().Text(i.Date.ToString("yyyy-MM-dd"));
                                table.Cell().Text(i.Category?.Title ?? "");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Developed by S.M. Shah Omar Asif, CSE, IUBAT © 2026. All rights reserved.")
                        .FontSize(10);
                });
            });

            return doc.GeneratePdf();
        }
    }
}
