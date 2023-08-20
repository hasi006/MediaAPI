using MediaAPI.Models;
using System.Collections;
using System.Security.Cryptography;

namespace MediaAPI.Services
{
    public class FileManagerService : IFileManagerService
    {
        private readonly string _uploadPath;
        private readonly long _maxFileSize;

        public FileManagerService(IConfiguration configuration)
        {
            _uploadPath = configuration["uploadPath"];
            _maxFileSize = Convert.ToInt64(configuration["maxFileSize"]);

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task SaveFiles(List<IFormFile> files)
        {
            if (files.Any(x => IsMaxSizeExceeded(x)))
            {
               throw new ApplicationException("Maximum file size exceeded");
            }

            foreach (var file in files)
            {
                var filePath = Path.Combine(_uploadPath, file.FileName);

                if (File.Exists(filePath) && AreFilesEqual(file.OpenReadStream(), filePath))
                {
                    throw new ApplicationException($"File not updated {file.FileName}");  // File not updated, return existing filename
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }

        public async Task<IEnumerable<FileDetail>> GetFileList(string search = "")
        {

            var files = Directory.GetFiles(_uploadPath);

            var fileList = files.Select(filePath =>
            {
                var fileInfo = new FileInfo(filePath);
                return new FileDetail
                {
                    FileName = Path.GetFileName(filePath),
                    Size = fileInfo.Length,
                    Date = fileInfo.LastWriteTime
                };
            });

            if (!string.IsNullOrEmpty(search))
            {
                fileList = fileList.Where(file =>
                    file.FileName.Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }

            return fileList;
        }

        private bool AreFilesEqual(Stream stream1, string filePath2)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream2 = File.OpenRead(filePath2))
                {
                    var hash1 = md5.ComputeHash(stream1);
                    var hash2 = md5.ComputeHash(stream2);
                    return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
                }
            }
        }

        private bool IsMaxSizeExceeded(IFormFile file)
        {
            var fileSize = file.Length;
            var fileSizeInMB = fileSize / (1024 * 1024);

            if (fileSizeInMB > _maxFileSize)
            {
                return true;
            }

            return false;
        }

    }    
}
