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

        public ServiceResponse<List<OrderDataRow>> ImportData(string token, List<string> selectedColumns)
        {
            var response = new ServiceResponse<List<OrderDataRow>>();
            if (!_fileStore.ContainsKey(token))
            {
                throw new Exception("Invalid token or file not found.");
            }

            var filePath = _fileStore[token];

            var orderDataRow = new List<OrderDataRow>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    int totalRows = worksheet.Dimension.End.Row;

                    // Loop through each row starting from the second row (data rows)
                    for (int row = 2; row <= totalRows; row++)
                    {
                        var previewRow = new OrderDataRow
                        {
                            PickupStoreNumber = worksheet.Cells[row, 3].Text,
                            PickupStoreName = worksheet.Cells[row, 4].Text,
                            PickupLat = worksheet.Cells[row, 5].Text,
                            PickupLon = worksheet.Cells[row, 6].Text,
                            PickupFormattedAddress = worksheet.Cells[row, 7].Text,
                            PickupContactFirstName = worksheet.Cells[row, 8].Text,
                            PickupContactLastName = worksheet.Cells[row, 9].Text,
                            PickupContactEmail = worksheet.Cells[row, 10].Text,
                            PickupContactMobileNumber = worksheet.Cells[row, 11].Text,
                            PickupEnableSmsNotification = worksheet.Cells[row, 12].Text,
                            PickupTime = worksheet.Cells[row, 13].Text,
                            PickupToleranceMin = worksheet.Cells[row, 14].Text,
                            PickupServiceTime = worksheet.Cells[row, 15].Text,
                            DeliveryStoreNumber = worksheet.Cells[row, 16].Text,
                            DeliveryStoreName = worksheet.Cells[row, 17].Text,
                            DeliveryLat = worksheet.Cells[row, 18].Text,
                            DeliveryLon = worksheet.Cells[row, 19].Text,
                            DeliveryFormattedAddress = worksheet.Cells[row, 20].Text,
                            DeliveryContactFirstName = worksheet.Cells[row, 21].Text,
                            DeliveryContactLastName = worksheet.Cells[row, 22].Text,
                            DeliveryContactEmail = worksheet.Cells[row, 23].Text,
                            DeliveryContactMobileNumber = worksheet.Cells[row, 24].Text,
                            DeliveryEnableSmsNotification = worksheet.Cells[row, 25].Text,
                            DeliveryTime = worksheet.Cells[row, 26].Text,
                            DeliveryToleranceMin = worksheet.Cells[row, 27].Text,
                            DeliveryServiceTime = worksheet.Cells[row, 28].Text,
                            OrderDetails = worksheet.Cells[row, 29].Text,
                            AssignedDriver = worksheet.Cells[row, 30].Text,
                            CustomerReference = worksheet.Cells[row, 31].Text,
                            Payer = worksheet.Cells[row, 32].Text,
                            Vehicle = worksheet.Cells[row, 33].Text,
                            Weight = worksheet.Cells[row, 34].Text,
                            Price = worksheet.Cells[row, 35].Text
                        };

                        orderDataRow.Add(previewRow);
                    }
                }
            }

            response.Data = orderDataRow;

            return response;
        }

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
            response.Token = token;
            response.Data = ExtractHeaders(tempFilePath);

            return response;
        }

        public async Task<ServiceResponse<ValidationResult>> ValidateFile(string token)
        {
            var response = new ServiceResponse<ValidationResult>();
            if (!_fileStore.ContainsKey(token))
                throw new Exception("Invalid or expired token.");

            var filePath = _fileStore[token];

            var validationData = ValidateAndPreview(filePath);

            response.Token = token;
            response.Data = validationData;
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

        private ValidationResult ValidateAndPreview(string filePath)
        {
            int totalRows = 0;
            var previewRow = new OrderDataRow();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    totalRows = worksheet.Dimension.End.Row - 1;

                    previewRow.PickupStoreNumber = worksheet.Cells[2, 3].Text;
                    previewRow.PickupStoreName = worksheet.Cells[2, 4].Text;
                    previewRow.PickupLat = worksheet.Cells[2, 5].Text;
                    previewRow.PickupLon = worksheet.Cells[2, 6].Text;
                    previewRow.PickupFormattedAddress = worksheet.Cells[2, 7].Text;
                    previewRow.PickupContactFirstName = worksheet.Cells[2, 8].Text;
                    previewRow.PickupContactLastName = worksheet.Cells[2, 9].Text;
                    previewRow.PickupContactEmail = worksheet.Cells[2, 10].Text;
                    previewRow.PickupContactMobileNumber = worksheet.Cells[2, 11].Text;
                    previewRow.PickupEnableSmsNotification = worksheet.Cells[2, 12].Text;
                    previewRow.PickupTime = worksheet.Cells[2, 13].Text;
                    previewRow.PickupToleranceMin = worksheet.Cells[2, 14].Text;
                    previewRow.PickupServiceTime = worksheet.Cells[2, 15].Text;
                    previewRow.DeliveryStoreNumber = worksheet.Cells[2, 16].Text;
                    previewRow.DeliveryStoreName = worksheet.Cells[2, 17].Text;
                    previewRow.DeliveryLat = worksheet.Cells[2, 18].Text;
                    previewRow.DeliveryLon = worksheet.Cells[2, 19].Text;
                    previewRow.DeliveryFormattedAddress = worksheet.Cells[2, 20].Text;
                    previewRow.DeliveryContactFirstName = worksheet.Cells[2, 21].Text;
                    previewRow.DeliveryContactLastName = worksheet.Cells[2, 22].Text;
                    previewRow.DeliveryContactEmail = worksheet.Cells[2, 23].Text;
                    previewRow.DeliveryContactMobileNumber = worksheet.Cells[2, 24].Text;
                    previewRow.DeliveryEnableSmsNotification = worksheet.Cells[2, 25].Text;
                    previewRow.DeliveryTime = worksheet.Cells[2, 26].Text;
                    previewRow.DeliveryToleranceMin = worksheet.Cells[2, 27].Text;
                    previewRow.DeliveryServiceTime = worksheet.Cells[2, 28].Text;
                    previewRow.OrderDetails = worksheet.Cells[2, 29].Text;
                    previewRow.AssignedDriver = worksheet.Cells[2, 30].Text;
                    previewRow.CustomerReference = worksheet.Cells[2, 31].Text;
                    previewRow.Payer = worksheet.Cells[2, 32].Text;
                    previewRow.Vehicle = worksheet.Cells[2, 33].Text;
                    previewRow.Weight = worksheet.Cells[2, 34].Text;
                    previewRow.Price = worksheet.Cells[2, 35].Text;
                }
            }

            return new ValidationResult
            {
                TotalRows = totalRows,
                PreviewRow = previewRow
            };
        }
    }
}