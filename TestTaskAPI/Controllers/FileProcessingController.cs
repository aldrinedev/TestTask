using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileProcessingController : ControllerBase
    {
        private static Dictionary<string, string> _fileStore = [];

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (Path.GetExtension(file.FileName) != ".xlsx")
                return BadRequest("Invalid file type. Please upload an Excel file.");

            var token = Guid.NewGuid().ToString();
            var tempFilePath = Path.GetTempFileName();

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _fileStore[token] = tempFilePath;

            var headers = ExtractHeaders(tempFilePath);

            return Ok(new { token, headers });
        }

        [HttpGet("validate-file/{token}")]
        public IActionResult ValidateFile(string token)
        {
            if (!_fileStore.ContainsKey(token))
                return BadRequest("Invalid or expired token.");

            var filePath = _fileStore[token];

            var validationData = ValidateAndPreview(filePath);

            return Ok(validationData);
        }

        [HttpPost("import/{token}")]
        public IActionResult ImportData(string token, [FromBody] List<string> selectedColumns)
        {
            if (!_fileStore.ContainsKey(token))
            {
                return BadRequest("Invalid token or file not found.");
            }

            var filePath = _fileStore[token];

            var excelData = ParseExcelFile(filePath);

            var jsonOutput = new Dictionary<string, List<object>>();

            foreach (var column in selectedColumns)
            {
                if (excelData.ContainsKey(column))
                {
                    jsonOutput[column] = new List<object>(excelData[column]);
                }
                else
                {
                    jsonOutput[column] = new List<object>();
                }
            }

            return Ok(new { Data = jsonOutput });
        }

        private Dictionary<string, List<object>> ParseExcelFile(string filePath)
        {
            var data = new Dictionary<string, List<object>>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; 
                var rowCount = worksheet.Dimension.Rows; 
                var colCount = worksheet.Dimension.Columns; 
                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    var header = worksheet.Cells[1, col].Text; 
                    headers.Add(header);
                    data[header] = new List<object>(); 
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        var header = headers[col - 1];
                        var value = worksheet.Cells[row, col].Value; 

                        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            data[header].Add(value);
                        }
                    }
                }
            }

            return data;
        }



        private List<string> ExtractHeaders(string filePath)
        {
            var headers = new List<string>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
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
                    totalRows = worksheet.Dimension.End.Row - 1;

                    for (int col = 3; col <= worksheet.Dimension.End.Column; col++)
                    {
                        var header = worksheet.Cells[1, col].Text;
                        var cellValue = worksheet.Cells[2, col].Text;
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