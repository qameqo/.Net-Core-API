using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Common.Interfaces
{
    public interface IManageImage
    {
        public Task<(bool, string)> UploadImage(string filepath, IFormFile File, string MsgErr);
        public bool DeleteImage(string filename, string folderPath, ref string MsgErr);
        public string FolderPath(string folder);
        public bool ValidateFile(IFormFile File, ref string MsgErr);
    }
}
