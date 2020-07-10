using SlothFreelance.Models;
using SlothFreelance.Unit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace SlothFreelance.AccountController
{
    public struct ImageSize
    {
        public int? width;
        public int? height;

        public ImageSize(int? _width, int? _height)
        {
            width = _width;
            height = _height;
        }
    }

    public class FileManager
    {
        public string AddImage(HttpPostedFileBase uploadImage, string serverPath, int? userId, ImageSize imageSize, bool sizeRequare = false)
        {
            if (uploadImage != null && userId != null)
            {
                string fileName = Path.GetFileName(uploadImage.FileName);
                string fileExtension = Path.GetExtension(uploadImage.FileName);
                DirectoryInfo directoryInfo = new DirectoryInfo(serverPath);

                if (!directoryInfo.Exists)
                {
                    Directory.CreateDirectory(serverPath);
                    directoryInfo = new DirectoryInfo(serverPath);
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    if (file.Name == fileName)
                    {
                        return "Файл с таким именем уже существует";
                    }
                }

                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                {
                    return "Неверный формат изображения";
                }

                if (imageSize.height != null && imageSize.width != null)
                {
                    if (!sizeRequare)
                    {
                        WebImage image = new WebImage(uploadImage.InputStream);

                        if (image.Height != imageSize.height)
                        {
                            return $"Размер изображения должен быть {imageSize.width}x{imageSize.height}";
                        }
                        else if (image.Width != imageSize.width)
                        {
                            return $"Размер изображения должен быть {imageSize.width}x{imageSize.height}";
                        }

                        uploadImage.SaveAs(serverPath + "/" + fileName);
                    }
                    else
                    {
                        WebImage image = new WebImage(uploadImage.InputStream);

                        if (image.Height < imageSize.height || image.Width < imageSize.width)
                        {
                            return $"Минимальный размер изображения: {imageSize.width}x{imageSize.height}";
                        }

                        if (image.Width > image.Height)
                        {
                            var leftRightCrop = (image.Width - image.Height) / 2;
                            image.Crop(0, leftRightCrop, 0, leftRightCrop);
                        }
                        else if (image.Width < image.Height)
                        {
                            var topBottomCrop = (image.Height - image.Width) / 2;
                            image.Crop(topBottomCrop, 0, topBottomCrop, 0);
                        }
                        image.Save(serverPath + "/" + fileName);
                    }
                }
                else
                {
                    uploadImage.SaveAs(serverPath + "/" + fileName);
                }
            }
            else
            {
                return "Операция не выполнена";
            }

            return null;
        }

        public void RemoveImage(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        public string AddFile(HttpPostedFileBase uploadFile, string serverPath, WorkOnTask work)
        {
            if (uploadFile != null)
            {
                string fileName = Path.GetFileName(uploadFile.FileName);
                DirectoryInfo directoryInfo = new DirectoryInfo(serverPath);

                if (!directoryInfo.Exists)
                {
                    Directory.CreateDirectory(serverPath);
                    directoryInfo = new DirectoryInfo(serverPath);
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    if (file.Name == fileName)
                    {
                        return "Файл с таким именем уже существует";
                    }
                }

                uploadFile.SaveAs(serverPath + "/" + fileName);
            }
            else
            {
                return "Операция не выполнена";
            }

            return null;
        }
    }
}