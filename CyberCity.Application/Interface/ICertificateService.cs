using CyberCity.Doman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateDetailedLinuxCertificatePdfAsync(
            string userId, 
            string courseId, 
            string studentName, 
            DateTime startDate,
            DateTime completionDate,
            int totalHours = 40,
            string instructorName = "CyberCity Academy",
            List<string> skillsLearned = null);
        Task<string> GenerateCertificateBase64Async(string userId, string courseId);

        Task<string> GenerateAsync(string userId, string courseId, string certificateType, decimal? completionPercentage);
        Task<IReadOnlyList<Certificate>> GetByStudentAsync(string studentId);
        Task<Certificate> GetByIdAsync(string certificateId);
        Task<byte[]> DownloadByIdAsync(string certificateId);
        Task<bool> VerifyAsync(string certificateId);
    }
}
