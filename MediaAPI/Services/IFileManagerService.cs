using MediaAPI.Models;

namespace MediaAPI.Services
{
    public interface IFileManagerService
    {
        Task SaveFiles(List<IFormFile> files);
        Task<IEnumerable<FileDetail>> GetFileList(string search = "");
    }
}
