using AcePacific.Common.Contract;
using AcePacific.Data.ViewModel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using ImageUploadResult = AcePacific.Busines.Services.ImageAccessor.ImageUploadResult;

namespace AcePacific.Busines.Services
{
    public interface IImageAccessor
    {
        Task<Response<ImageUploadResult>> UploadImageAsync(IFormFile file);
        Task<Response<string>> DeletePhoto(string publicId);
        Task<Response<ImageUploadResult>> UploadBase64ImageAsync(string base64Image);
    }
    public class ImageAccessor : IImageAccessor
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySetting _cloudinarySetting;
        public ImageAccessor(CloudinarySetting cloudinarySetting)
        {
            _cloudinarySetting = cloudinarySetting;

            var account = new Account
            {
                Cloud = _cloudinarySetting.CloudName,
                ApiKey = _cloudinarySetting.CloudinaryApiKey,
                ApiSecret = _cloudinarySetting.CloudinaryApiSecret
            };

            _cloudinary = new Cloudinary(account);
        }

        public async Task<Response<string>> DeletePhoto(string publicId)
        {
            Response<string>? response;
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                response = Response<string>.Success(result.Result == "Deleted" ? result.Result : null);
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);

        }

        public async Task<Response<ImageUploadResult>> UploadImageAsync(IFormFile file)
        {
            var response = Response<ImageUploadResult>.Failed(string.Empty);
            try
            {

                if (file.Length > 0)
                {
                    await using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        //Transformation = new Transformation().Height(500).Width(500)
                    };
                    var uploadResult = _cloudinary.Upload(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        response = Response<ImageUploadResult>.Failed(uploadResult.Error.Message);
                    }

                    response = Response<ImageUploadResult>.Success(new ImageUploadResult()
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.ToString()
                    });

                }
            }
            catch (Exception ex)
            {
                response = Response<ImageUploadResult>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }

        public async Task<Response<ImageUploadResult>> UploadBase64ImageAsync(string base64Image)
        {
            var response = Response<ImageUploadResult>.Failed(string.Empty);
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                using (var stream = new MemoryStream(imageBytes))
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription("base64image", stream)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        response = Response<ImageUploadResult>.Success(new ImageUploadResult()
                        {
                            PublicId = uploadResult.PublicId,
                            ImageUrl = uploadResult.SecureUrl.ToString()
                        });
                    }
                    else
                    {
                        response = Response<ImageUploadResult>.Failed($"Image upload failed: {uploadResult.Error.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                response = Response<ImageUploadResult>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public class ImageUploadResult
        {
            public string PublicId { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
