using ClosedXML.Excel;
using Lab10_Lazarinos.Application.Interfaces.Services;
using System.IO;

namespace Lab10_Lazarinos.Infrastructure.Services
{
    public class ExcelGeneratorService : IExcelGeneratorService
    {
        public byte[] GenerateTicketReport(IEnumerable<TicketReportDto> tickets)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tickets");
                var currentRow = 1;

                // Encabezados
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Título";
                worksheet.Cell(currentRow, 3).Value = "Estado";
                worksheet.Cell(currentRow, 4).Value = "Fecha Creación";

                // Datos
                foreach (var ticket in tickets)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = ticket.Id;
                    worksheet.Cell(currentRow, 2).Value = ticket.Title;
                    worksheet.Cell(currentRow, 3).Value = ticket.Status;
                    worksheet.Cell(currentRow, 4).Value = ticket.CreatedAt;
                }

                // Guardar en memoria
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        // public byte[] GenerateUserReport(IEnumerable<UserReportDto> users)
        // {
        //     // ... lógica para el segundo reporte ...
        // }
    }
}