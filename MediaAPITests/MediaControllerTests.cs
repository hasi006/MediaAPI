using MediaAPI.Controllers;
using MediaAPI.Models;
using MediaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace MediaAPITests
{
    [TestClass]
    public class MediaControllerTests
    {
        private MediaController _controller;
        private Mock<IFileManagerService> _mockFileManagerService;
        private Mock<IConfiguration> _mockConfiguration;

        [TestInitialize]
        public void Initialize()
        {
            _mockFileManagerService = new Mock<IFileManagerService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new MediaController(_mockFileManagerService.Object, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task UploadMedia_ValidFiles_ReturnsOk()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                new FormFile(null, 0, 0, "file1", "file1.txt")
            };

            _mockFileManagerService.Setup(service => service.SaveFiles(files)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadMedia(files);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task UploadMedia_NoFiles_ReturnsBadRequest()
        {
            // Arrange
            var files = new List<IFormFile>(); // No files

            // Act
            var result = await _controller.UploadMedia(files);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UploadMedia_Exception_ReturnsBadRequest()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                new FormFile(null, 0, 0, "file1", "file1.txt")
            };

            _mockFileManagerService.Setup(service => service.SaveFiles(files)).Throws(new Exception("Test Exception"));

            // Act
            var result = await _controller.UploadMedia(files);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetFileList_ReturnsOk()
        {
            // Arrange
            var fileList = new List<FileDetail>
            {
                new FileDetail { FileName = "file1.txt", Size = 100, Date = DateTime.Now },
                new FileDetail { FileName = "file2.txt", Size = 200, Date = DateTime.Now }
            };

            _mockFileManagerService.Setup(service => service.GetFileList(It.IsAny<string>())).ReturnsAsync(fileList);

            // Act
            var result = await _controller.GetFileList();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedFileList = okResult.Value as List<FileDetail>;
            Assert.AreEqual(fileList.Count, returnedFileList.Count);
        }

        [TestMethod]
        public async Task GetFileList_WithSearch_ReturnsOk()
        {
            // Arrange
            var searchQuery = "file1";
            var fileList = new List<FileDetail>
            {
                new FileDetail { FileName = "file1.txt", Size = 100, Date = DateTime.Now }
            };

            _mockFileManagerService.Setup(service => service.GetFileList(searchQuery)).ReturnsAsync(fileList);

            // Act
            var result = await _controller.GetFileList(searchQuery);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedFileList = okResult.Value as List<FileDetail>;
            Assert.AreEqual(fileList.Count, returnedFileList.Count);
        }

        [TestMethod]
        public async Task GetFileList_Exception_ReturnsBadRequest()
        {
            // Arrange
            _mockFileManagerService.Setup(service => service.GetFileList(It.IsAny<string>())).Throws(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetFileList();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

    }
}