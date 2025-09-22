using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Serein.Candle.Infrastructure.Services
{
    public class CloudinaryImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _cloudinarySettings;

        public CloudinaryImageService(IOptions<CloudinarySettings> config)
        {
            _cloudinarySettings = config.Value;
            var acc = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret); 
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<List<string>> UploadImagesAsync(IFormFileCollection files)
        {
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    await using var stream = file.OpenReadStream();

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = Guid.NewGuid().ToString(),
                        UploadPreset = "upload-Serein",
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK && uploadResult.SecureUrl != null)
                    {
                        imageUrls.Add(uploadResult.SecureUrl.ToString());
                    }
                    else
                    {
                        throw new Exception($"Upload failed: {uploadResult.Error?.Message}");
                    }
                }
            }

            return imageUrls;
        }

    }
}
