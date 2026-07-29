using BLL.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.ImplementationService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly List<string> extintions = new List<string> { ".jpg", ".png", ".jpeg" };
        private const int maxFileSize = 2 * 1024 * 1024; 
        public async Task<string?> Upload(IFormFile file, string folderName)
        {
            var extintion = Path.GetExtension(file.FileName).ToLower();
            if (!extintions.Contains(extintion.ToLower())) return null;

            if (file.Length > maxFileSize || file.Length == 0) return null;

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName);

            string fileName = $"{Guid.NewGuid().ToString()}_{file.FileName}";

            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/files/{folderName}/{fileName}";

        }

        public bool Delete(string filePath)
        {
            if (Directory.Exists(filePath)) return false;
            File.Delete(filePath);
            return true;
        }
    }
}
