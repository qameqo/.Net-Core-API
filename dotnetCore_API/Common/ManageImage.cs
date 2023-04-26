using dotnetCore_API.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetCore_API.Common
{
    public class ManageImage : IManageImage
    {
        private readonly IWebHostEnvironment _env;

        public ManageImage(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<(bool,string)> UploadImage(string filepath, IFormFile File,string MsgErr)
        {
            var result = false;
            try
            {
                bool resValidate = ValidateFile(File,ref MsgErr);
                if (!resValidate && !string.IsNullOrEmpty(MsgErr))
                {
                    throw new Exception(MsgErr);
                }
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                MsgErr = ex.Message.Trim();
            }
            return (result,MsgErr);
        }

        public bool DeleteImage(string filename,string folderPath, ref string MsgErr)
        {
            var result = false;
            try
            {
                var imagePath = Path.Combine(folderPath, filename);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                MsgErr = ex.Message.Trim();
            }
            return result;
        }
        public string FolderPath(string folder)
        {
            var folderPath = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public bool ValidateFile(IFormFile File,ref string MsgErr)
        {
            bool result = false;

            try
            {
                long fileSize = File.Length;
                string contentType = File.ContentType;

                if (fileSize <= 2000000)
                {
                    if (contentType == "image/jpeg" || contentType == "image/png")
                    {
                        MsgErr = "";
                        result = true;
                    }
                    else 
                    {
                        MsgErr = "The format File is not jpeg or png.";
                    }
                }
                else
                {
                    MsgErr = "File size more than 2mb";
                }
            }
            catch (Exception ex)
            {
                result = false;
                MsgErr = ex.Message.Trim();
            }
            return result;
        }
    }
}
