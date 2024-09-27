using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestTask.Models;
using TestTaskAPI.Models;

namespace TestTaskAPI.Services
{
    public class FileProcessService : IFileProcessService
    {
        private static Dictionary<string, string> _fileStore = [];
        public async Task<ServiceResponse<List<string>>> UploadFile(IFormFile file)
        {
            var response = new ServiceResponse<List<string>>();
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            if (Path.GetExtension(file.FileName) != ".xlsx")
                throw new Exception("Invalid file type. Please upload an Excel file.");

            var token = Guid.NewGuid().ToString();
            var tempFilePath = Path.GetTempFileName();

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _fileStore[token] = tempFilePath;

            response.Data = ExtractHeaders(tempFilePath);

            return response;
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
    }
}