using DefNotEbay_API.DTOs.Export;

namespace DefNotEbay_API.Services.Interfaces
{
    public interface IExportService
    {
        Task<ExportResult> ExportAsync(string format = "json",int? sellerId = null,DateTime? start = null,DateTime? end = null);
    }

    public class ExportResult
    {
        public required byte[] Content { get; init; }
        public required string ContentType { get; init; }
        public required string FileName { get; init; }
    }
}
