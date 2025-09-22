using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Application.Interface;
    using CyberCity.Doman.Models;
    using CyberCity.Infrastructure;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
namespace CyberCity.Application.Implement
{
    public class CertificateService : ICertificateService
    {
        private readonly CertificateRepo _certificateRepo;

        public CertificateService(CertificateRepo certificateRepo)
        {
            _certificateRepo = certificateRepo;
            // Ensure QuestPDF is set to community license
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateDetailedLinuxCertificatePdfAsync(
            Guid userId, 
            Guid courseId, 
            string studentName, 
            DateTime startDate,
            DateTime completionDate,
            int totalHours = 40,
            string instructorName = "CyberCity Academy",
            List<string> skillsLearned = null)
        {
            skillsLearned ??= new List<string>
            {
                "Linux Command Line Interface",
                "File System Navigation",
                "Process Management", 
                "User & Permission Management",
                "Shell Scripting Basics",
                "System Administration"
            };

            var pdfBytes = await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                        page.Content()
                            .Stack(stack =>
                            {
                                // Header
                                stack.Item()
                                    .Background(Colors.Green.Darken2) // Linux green theme
                                    .Padding(20)
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Column(col =>
                                            {
                                                col.Item().Text("CYBERCITY ACADEMY")
                                                    .FontSize(24)
                                                    .Bold()
                                                    .FontColor(Colors.White);
                                                col.Item().Text("Linux Training & Certification Program")
                                                    .FontSize(12)
                                                    .FontColor(Colors.White);
                                            });
                                        
                                        row.ConstantItem(100)
                                            .AlignCenter()
                                            .AlignMiddle()
                                            .Column(logoCol =>
                                            {
                                                logoCol.Item()
                                                    .AlignCenter()
                                                    .Text("🐧") // Penguin for Linux
                                                    .FontSize(32);
                                                logoCol.Item()
                                                    .AlignCenter()
                                                    .Text("LINUX")
                                                    .FontSize(12)
                                                    .Bold()
                                                    .FontColor(Colors.White);
                                            });
                                    });

                                // Main content
                                stack.Item()
                                    .Padding(30)
                                    .Column(col =>
                                    {
                                        col.Spacing(15);
                                        
                                        col.Item()
                                            .AlignCenter()
                                            .Text("CERTIFICATE OF COMPLETION")
                                            .FontSize(28)
                                            .Bold()
                                            .FontColor(Colors.Green.Darken3);
                                        
                                        col.Item()
                                            .AlignCenter()
                                            .Width(400)
                                            .Height(2)
                                            .Background(Colors.Green.Medium);
                                        
                                        col.Item().Height(15);
                                        
                                        col.Item()
                                            .AlignCenter()
                                            .Text("This certifies that")
                                            .FontSize(14)
                                            .Italic();
                                        
                                        col.Item()
                                            .AlignCenter()
                                            .Text(studentName)
                                            .FontSize(24)
                                            .Bold()
                                            .FontColor(Colors.Green.Darken3);
                                        
                                        col.Item()
                                            .AlignCenter()
                                            .Text("has successfully completed")
                                            .FontSize(14);
                                        
                                        col.Item()
                                            .AlignCenter()
                                            .Border(2)
                                            .BorderColor(Colors.Green.Medium)
                                            .Padding(12)
                                            .Background(Colors.Green.Lighten4)
                                            .Text("LINUX FUNDAMENTALS COURSE")
                                            .FontSize(18)
                                            .Bold()
                                            .FontColor(Colors.Green.Darken3);
                                        
                                        // Course details
                                        col.Item()
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Medium)
                                            .Padding(15)
                                            .Background(Colors.Grey.Lighten4)
                                            .Column(detailCol =>
                                            {
                                                detailCol.Item()
                                                    .Text("Course Details:")
                                                    .FontSize(12)
                                                    .Bold()
                                                    .FontColor(Colors.Green.Darken2);
                                                
                                                detailCol.Item()
                                                    .Row(detailRow =>
                                                    {
                                                        detailRow.RelativeItem()
                                                            .Column(leftDetail =>
                                                            {
                                                                leftDetail.Item().Text($"Duration: {totalHours} hours").FontSize(10);
                                                                leftDetail.Item().Text($"Start Date: {startDate:MMM dd, yyyy}").FontSize(10);
                                                                leftDetail.Item().Text($"Completion: {completionDate:MMM dd, yyyy}").FontSize(10);
                                                            });
                                                        
                                                        detailRow.RelativeItem()
                                                            .Column(rightDetail =>
                                                            {
                                                                rightDetail.Item().Text("Skills Acquired:").FontSize(10).Bold();
                                                                foreach (var skill in skillsLearned.Take(3))
                                                                {
                                                                    rightDetail.Item().Text($"• {skill}").FontSize(9);
                                                                }
                                                                if (skillsLearned.Count > 3)
                                                                {
                                                                    rightDetail.Item().Text($"• And {skillsLearned.Count - 3} more skills...").FontSize(9).Italic();
                                                                }
                                                            });
                                                    });
                                            });
                                        
                                        // Footer
                                        col.Item()
                                            .Row(footerRow =>
                                            {
                                                footerRow.RelativeItem()
                                                    .Column(leftFooter =>
                                                    {
                                                        leftFooter.Item().Text($"Certificate ID: LINUX-{courseId.ToString().Substring(0, 8).ToUpper()}").FontSize(10).Bold();
                                                        leftFooter.Item().Text($"Student ID: {userId.ToString().Substring(0, 8).ToUpper()}").FontSize(9);
                                                        leftFooter.Item().Text($"Issued: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(9);
                                                    });
                                                
                                                footerRow.RelativeItem()
                                                    .AlignRight()
                                                    .Column(rightFooter =>
                                                    {
                                                        rightFooter.Item()
                                                            .AlignCenter()
                                                            .Border(1)
                                                            .BorderColor(Colors.Grey.Medium)
                                                            .Width(120)
                                                            .Height(40)
                                                            .AlignMiddle()
                                                            .Text("🔒 Verified")
                                                            .FontSize(10)
                                                            .Bold()
                                                            .FontColor(Colors.Green.Darken2);
                                                        
                                                        rightFooter.Item()
                                                            .AlignCenter()
                                                            .Text(instructorName)
                                                            .FontSize(11)
                                                            .Bold();
                                                    });
                                            });
                                    });
                                
                                stack.Item()
                                    .Background(Colors.Green.Darken2)
                                    .Height(8);
                            });
                    });
                });
                
                return document.GeneratePdf();
            });
            
            return pdfBytes;
        }

        public async Task<string> GenerateCertificateBase64Async(Guid userId, Guid courseId)
        {
            var pdfBytes = await GenerateDetailedLinuxCertificatePdfAsync(
                userId, 
                courseId, 
                "Student Name", // Default placeholder
                DateTime.Now.AddDays(-30), // Default start date
                DateTime.Now, // Default completion date
                40, // Default hours
                "CyberCity Academy", // Default instructor
                null // Default skills
            );
            return Convert.ToBase64String(pdfBytes);
        }

        public async Task<Guid> GenerateAsync(Guid userId, Guid courseId, string certificateType, decimal? completionPercentage)
        {
            var pdfBytes = await GenerateDetailedLinuxCertificatePdfAsync(
                userId, 
                courseId, 
                "Student Name", // Default placeholder
                DateTime.Now.AddDays(-30), // Default start date
                DateTime.Now, // Default completion date
                40, // Default hours
                "CyberCity Academy", // Default instructor
                null // Default skills
            );

            var certificate = new Certificate
            {
                Uid = Guid.NewGuid(),
                UserUid = userId,
                CourseUid = courseId,
                CertificateType = certificateType,
                CompletionPercentage = completionPercentage,
                FileUrl = null,
                IssuedAt = DateTime.Now
            };

            await _certificateRepo.CreateAsync(certificate);
            return certificate.Uid;
        }

        public async Task<IReadOnlyList<Certificate>> GetByStudentAsync(Guid studentId)
        {
            var list = await _certificateRepo.GetByUserUidAsync(studentId);
            return list;
        }

        public async Task<Certificate> GetByIdAsync(Guid certificateId)
        {
            return await _certificateRepo.GetByIdAsync(certificateId);
        }

        public async Task<byte[]> DownloadByIdAsync(Guid certificateId)
        {
            var cert = await _certificateRepo.GetByIdAsync(certificateId);
            if (cert == null) return Array.Empty<byte>();
            // For now, regenerate dynamically. If FileUrl stored, you can fetch instead.
            return await GenerateDetailedLinuxCertificatePdfAsync(
                cert.UserUid, 
                cert.CourseUid, 
                "Student Name", // Default placeholder
                DateTime.Now.AddDays(-30), // Default start date
                DateTime.Now, // Default completion date
                40, // Default hours
                "CyberCity Academy", // Default instructor
                null // Default skills
            );
        }

        public async Task<bool> VerifyAsync(Guid certificateId)
        {
            var cert = await _certificateRepo.GetByIdAsync(certificateId);
            return cert != null;
        }
    }
}
