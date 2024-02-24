using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace netcore.Controllers
{
    [ApiController]
    //[Route("api/test")]
    [Route("api/[controller]/[action]")]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IConfiguration configuration;

        private readonly IHostEnvironment _hostEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(ILogger<UploadController> logger, IConfiguration _configuration, IHostEnvironment hostEnvironment, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            configuration = _configuration;
            _hostEnvironment = hostEnvironment;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 服务器配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<ServerViewModel> Server()
        {
            var data = new ServerViewModel()
            {
                EnvironmentName = _hostEnvironment.EnvironmentName,
                OSArchitecture = RuntimeInformation.OSArchitecture.ObjToString(),
                ContentRootPath = _hostEnvironment.ContentRootPath,
                WebRootPath = _webHostEnvironment.WebRootPath,
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                MemoryFootprint = (Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + " MB",
                WorkingTime = DateHelper.TimeSubTract(DateTime.Now, Process.GetCurrentProcess().StartTime)
            };
            return MessageModel<ServerViewModel>.Success("成功", data);
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> up(IFormFile file, [FromQuery] string uploadName, [FromQuery] string uploadPass)
        {
            try
            {
                if(!(UploadInfo.uploadName.Equals(uploadName) && UploadInfo.uploadPass.Equals(uploadPass)))
                    return MessageModel<string>.Fail("账号密码验证错误");

                if (file == null || file.Length == 0)
                    return MessageModel<string>.Fail("请选择要上传的文件");

                string year = DateTime.Now.ToString("yyyy");
                string date = DateTime.Now.ToString("yyyyMMdd");
                string dic = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", year, date);
                if (!Directory.Exists(dic))
                    Directory.CreateDirectory(dic);

                var suffix = Path.GetExtension(file.FileName);
                var name = $"{date}_{StringHelper.GetGUID()}{suffix}";

                if (!UploadInfo.allowTypes.Contains(suffix))
                {
                    return MessageModel<string>.Fail($"文件类型不支持:{suffix}");
                }

                var path = Path.Combine(dic, name);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var url = $"{UploadInfo.uploadUrl}/{year}/{date}/{name}";

                return MessageModel<string>.Success("文件上传成功", url);
            }
            catch (Exception ex)
            {
                return MessageModel<string>.Fail($"文件上传异常:{ex.Message}");
            }
        }

        [HttpPost]
        public async Task<MessageModel<string>> upBase64Image(UploadBase64Dto  data)
        {

            try
            {

                if (!(UploadInfo.uploadName.Equals(data.uploadName) && UploadInfo.uploadPass.Equals(data.uploadPass)))
                    return MessageModel<string>.Fail("账号密码验证错误"); 

                string year = DateTime.Now.ToString("yyyy");
                string date = DateTime.Now.ToString("yyyyMMdd");
                string dic = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", year, date);
                if (!Directory.Exists(dic))
                    Directory.CreateDirectory(dic);

                var suffix = ".png";
                var name = $"{date}_{StringHelper.GetGUID()}{suffix}";

                if (!UploadInfo.allowTypes.Contains(suffix))
                {
                    return MessageModel<string>.Fail($"文件类型不支持:{suffix}");
                }

                var path = Path.Combine(dic, name);

                string base64Data = data.base64Image.Substring(data.base64Image.IndexOf(",") + 1);

                byte[] imageBytes = Convert.FromBase64String(base64Data); 

                await System.IO.File.WriteAllBytesAsync(path, imageBytes);

                var url = $"{UploadInfo.uploadUrl}/{year}/{date}/{name}";


                return MessageModel<string>.Success("文件上传成功", url);
            }
            catch (Exception ex)
            {
                return MessageModel<string>.Fail($"文件上传异常:{ex.Message}");
            }



        }
    }
}