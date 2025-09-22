using CyberCity.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromQuery] Guid userId, [FromQuery] Guid courseId, [FromQuery] string certificateType = "completion", [FromQuery] decimal? completionPercentage = null)
        {
            var id = await _certificateService.GenerateAsync(userId, courseId, certificateType, completionPercentage);
            return Ok(new { id });
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(Guid studentId)
        {
            var list = await _certificateService.GetByStudentAsync(studentId);
            return Ok(list);
        }

        [HttpGet("{certificateId}")]
        public async Task<IActionResult> GetDetail(Guid certificateId)
        {
            var cert = await _certificateService.GetByIdAsync(certificateId);
            if (cert == null) return NotFound();
            return Ok(cert);
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] Guid userId, [FromQuery] Guid courseId, [FromQuery] string studentName = "Student Name", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? completionDate = null, [FromQuery] int totalHours = 40, [FromQuery] string instructorName = "CyberCity Academy")
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var completion = completionDate ?? DateTime.Now;
            var bytes = await _certificateService.GenerateDetailedLinuxCertificatePdfAsync(userId, courseId, studentName, start, completion, totalHours, instructorName, null);
            var fileName = $"certificate_{userId}_{courseId}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        [HttpGet("{certificateId}/download")]
        public async Task<IActionResult> DownloadById(Guid certificateId)
        {
            var bytes = await _certificateService.DownloadByIdAsync(certificateId);
            if (bytes == null || bytes.Length == 0) return NotFound();
            var fileName = $"certificate_{certificateId}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        [HttpGet("inline-base64")]
        public async Task<IActionResult> GetInlineBase64([FromQuery] Guid userId, [FromQuery] Guid courseId, [FromQuery] string studentName = "Student Name", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? completionDate = null, [FromQuery] int totalHours = 40, [FromQuery] string instructorName = "CyberCity Academy")
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var completion = completionDate ?? DateTime.Now;
            var base64 = await _certificateService.GenerateCertificateBase64Async(userId, courseId);
            return Ok(new { base64 });
        }

        [HttpGet("inline")]
        public async Task<IActionResult> GetInline([FromQuery] Guid userId, [FromQuery] Guid courseId, [FromQuery] string studentName = "Student Name", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? completionDate = null, [FromQuery] int totalHours = 40, [FromQuery] string instructorName = "CyberCity Academy")
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var completion = completionDate ?? DateTime.Now;
            var bytes = await _certificateService.GenerateDetailedLinuxCertificatePdfAsync(userId, courseId, studentName, start, completion, totalHours, instructorName, null);
            Response.Headers["Content-Disposition"] = "inline; filename=certificate.pdf";
            return File(bytes, "application/pdf");
        }

        [HttpGet("linux-certificate")]
        public async Task<IActionResult> GenerateLinuxCertificate(
            [FromQuery] Guid userId, 
            [FromQuery] Guid courseId, 
            [FromQuery] string studentName, 
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime completionDate,
            [FromQuery] int totalHours = 40,
            [FromQuery] string instructorName = "CyberCity Academy",
            [FromQuery] List<string> skillsLearned = null)
        {
            var bytes = await _certificateService.GenerateDetailedLinuxCertificatePdfAsync(
                userId, courseId, studentName, startDate, completionDate, totalHours, instructorName, skillsLearned);
            var fileName = $"linux_certificate_{studentName}_{courseId}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        [HttpGet("{certificateId}/verify")]
        public async Task<IActionResult> Verify(Guid certificateId)
        {
            var isValid = await _certificateService.VerifyAsync(certificateId);
            return Ok(new { valid = isValid });
        }
    }
}


