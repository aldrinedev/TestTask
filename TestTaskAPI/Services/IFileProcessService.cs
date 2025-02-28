using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTask.Models;
using TestTaskAPI.Models;

namespace TestTaskAPI.Services
{
    public interface IFileProcessService
    {
        Task<ServiceResponse<List<string>>> UploadFile(IFormFile file);
        Task<ServiceResponse<ValidationResult>> ValidateFile(string token);
        ServiceResponse<List<OrderDataRow>> ImportData(string token, List<string> selectedColumns);
    }
}