using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TestTaskAPI.Models;
using TestTaskAPI.Services;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileProcessingController : ControllerBase
    {
        private readonly IFileProcessService _fileProcessService;

        public FileProcessingController(IFileProcessService fileProcessService)
        {
            _fileProcessService = fileProcessService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<ServiceResponse<List<string>>>> UploadFile(IFormFile file)
        {
            return Ok(await _fileProcessService.UploadFile(file));
        }

        [HttpGet("validate-file/{token}")]
        public async Task<ActionResult<ServiceResponse<ValidationResult>>> ValidateFile(string token)
        {
            return Ok(await _fileProcessService.ValidateFile(token));
        }

        [HttpPost("import/{token}")]
        public IActionResult ImportData(string token, [FromBody] List<string> selectedColumns)
        {
            return Ok(_fileProcessService.ImportData(token, selectedColumns));
        }
    }
}