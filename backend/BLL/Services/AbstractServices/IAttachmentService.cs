using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AbstractServices
{
    public interface IAttachmentService
    {
        public Task<string> Upload(IFormFile image, string folderName);

        public bool Delete(string filePath);
    }
}
