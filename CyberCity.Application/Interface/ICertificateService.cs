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
            Guid userId, 
            Guid courseId, 
            string studentName, 
            DateTime startDate,
            DateTime completionDate,
            int totalHours = 40,
            string instructorName = "CyberCity Academy",
            List<string> skillsLearned = null);
        Task<string> GenerateCertificateBase64Async(Guid userId, Guid courseId);

        Task<Guid> GenerateAsync(Guid userId, Guid courseId, string certificateType, decimal? completionPercentage);
        Task<IReadOnlyList<Certificate>> GetByStudentAsync(Guid studentId);
        Task<Certificate> GetByIdAsync(Guid certificateId);
        Task<byte[]> DownloadByIdAsync(Guid certificateId);
        Task<bool> VerifyAsync(Guid certificateId);
    }
}
