using MediaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MediaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IFileManagerService _fileSaverService;
        private readonly IConfiguration _config;

        public MediaController(IFileManagerService fileSaverService, IConfiguration config)
        {
            _fileSaverService = fileSaverService;
            _config = config;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadMedia([FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("No files Avialble");

                }

                await _fileSaverService.SaveFiles(files);

                return Ok("File Uploaded Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("FileList")]
        public async Task<IActionResult> GetFileList([FromQuery]string search="")
        {
            try
            {
               var fileList = await _fileSaverService.GetFileList(search);

                return Ok(fileList);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }        
    }
}
