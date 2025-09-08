using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CyberCity.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql.BackendMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class CloudinaryService :ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "CyberCityImgs_Folder" // thư mục trong Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
