using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private static Dictionary<string, string> _fileStore = new Dictionary<string, string>(); // Store file paths by token

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (Path.GetExtension(file.FileName) != ".xlsx")
                return BadRequest("Invalid file type. Please upload an Excel file.");

            // Generate a unique token (GUID)
            var token = Guid.NewGuid().ToString();
            var tempFilePath = Path.GetTempFileName(); // Create a temporary file path

            // Save the uploaded file to the temporary location
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Store the file path associated with the token
            _fileStore[token] = tempFilePath;

            // Extract headers from the Excel file
            var headers = ExtractHeaders(tempFilePath);

            // Return the token and the parsed headers
            return Ok(new { Token = token, Headers = headers });
        }

        [HttpGet("validate-file/{token}")]
        public IActionResult ValidateFile(string token)
        {
            if (!_fileStore.ContainsKey(token))
                return BadRequest("Invalid or expired token.");

            var filePath = _fileStore[token];

            // Extract total rows and the first row of data
            var validationData = ValidateAndPreview(filePath);

            return Ok(validationData);
        }

        private List<string> ExtractHeaders(string filePath)
        {
            var headers = new List<string>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    // Start from the 3rd column (skipping "Order Type" and "Import")
                    for (int col = 3; col <= worksheet.Dimension.End.Column; col++)
                    {
                        var header = worksheet.Cells[1, col].Text;
                        headers.Add(header);
                    }
                }
            }

            return headers;
        }

        private object ValidateAndPreview(string filePath)
        {
            int totalRows = 0;
            Dictionary<string, object> firstRowData = new Dictionary<string, object>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    totalRows = worksheet.Dimension.End.Row - 1;  // Exclude the header row

                    // Start processing from the 3rd column (skipping "Order Type" and "Import")
                    for (int col = 3; col <= worksheet.Dimension.End.Column; col++)
                    {
                        var header = worksheet.Cells[1, col].Text;
                        var cellValue = worksheet.Cells[2, col].Text;  // First data row
                        firstRowData[header] = cellValue;
                    }
                }
            }

            return new
            {
                TotalRows = totalRows,
                PreviewRow = firstRowData
            };
        }
    }
}