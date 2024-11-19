using Microsoft.AspNetCore.Mvc;
using netcore.Dto;
using SqlSugar;
using System.Data;
using System.Diagnostics; 
using System.Runtime.InteropServices; 

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
        /// �����ļ�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<string> clean()
        {

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.MySql,
                ConnectionString = UploadInfo.connectionStr,
                IsAutoCloseConnection = true
            });
            List<string> dataBaseFiles = new List<string>();
            var picShopList = db.Queryable<AppShopInfo>().Select(t=> new { t.appIcon ,t.appUrl}).ToList();
            foreach (var item in picShopList)
            {
                dataBaseFiles.Add(item.appIcon);
                dataBaseFiles.Add(item.appUrl);
            }
            var picCustomerList = db.Queryable<NightscoutCustomer>().Select(t => new { t.logo }).ToList();
            foreach(var item in picCustomerList)
            {
                dataBaseFiles.Add(item.logo);
            }

            var localFilesMsg = getFileList(UploadInfo.uploadName, UploadInfo.uploadPass);
            if (localFilesMsg.success)
            {
                foreach (var item in localFilesMsg.response)
                {
                    if (!dataBaseFiles.Contains(item.netPath)) 
                    {
                        LogHelper.logApp.Info($"ɾ���ļ�:{item.filePath}");
                        LogHelper.logApp.Info($"����·��:{item.netPath}");
                        System.IO.File.Delete(item.filePath);
                    }
                }
            }
            
            return MessageModel<string>.Success("�ɹ�");
        }
        /// <summary>
        /// ��ȡ�ļ��б�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<List<CleanFileDto>> getFileList([FromQuery] string uploadName, [FromQuery] string uploadPass)
        {
            if (!(UploadInfo.uploadName.Equals(uploadName) && UploadInfo.uploadPass.Equals(uploadPass)))
                return MessageModel<List<CleanFileDto>>.Fail("�˺�������֤����");

            List<CleanFileDto> list = new List<CleanFileDto>();
            var dicPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            var files = Directory.GetFiles(dicPath);
            var dics = Directory.GetDirectories(dicPath);
            getFileList(dicPath, list, files, dics);

            return MessageModel<List<CleanFileDto>>.Success("�ɹ�", list);
        }
        private void getFileList(string rootPath,List<CleanFileDto> list, string[] files, string[] dics)
        {
            if(files.Length > 0)
            {
                foreach(var file in files)
                {
                    var formatFile = Path.GetFullPath(file);
                    var url = formatFile.Replace(rootPath, "").Replace($@"\","/");
                    list.Add(new CleanFileDto {  netPath= UploadInfo.uploadUrl + url,filePath = formatFile });
                }
            }
            if (dics.Length > 0)
            {
                foreach (var dic in dics)
                {
                    var filesNext = Directory.GetFiles(dic);
                    var dicsNext = Directory.GetDirectories(dic);
                    getFileList(rootPath, list, filesNext, dicsNext);
                }
            }
        }

        /// <summary>
        /// ������������Ϣ
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
            return MessageModel<ServerViewModel>.Success("�ɹ�", data);
        }
        /// <summary>
        /// �ļ��ϴ�
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> up(IFormFile file, [FromQuery] string uploadName, [FromQuery] string uploadPass)
        {
            try
            {
                if(!(UploadInfo.uploadName.Equals(uploadName) && UploadInfo.uploadPass.Equals(uploadPass)))
                    return MessageModel<string>.Fail("�˺�������֤����");

                if (file == null || file.Length == 0)
                    return MessageModel<string>.Fail("��ѡ��Ҫ�ϴ����ļ�");

                string year = DateTime.Now.ToString("yyyy");
                string date = DateTime.Now.ToString("yyyyMMdd");
                string dic = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", year, date);
                if (!Directory.Exists(dic))
                    Directory.CreateDirectory(dic);

                var suffix = Path.GetExtension(file.FileName);
                var name = $"{date}_{StringHelper.GetGUID()}{suffix}";

                if (!UploadInfo.allowTypes.Contains(suffix))
                {
                    return MessageModel<string>.Fail($"�ļ����Ͳ�֧��:{suffix}");
                }

                var path = Path.Combine(dic, name);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var url = $"{UploadInfo.uploadUrl}/{year}/{date}/{name}";

                return MessageModel<string>.Success("�ļ��ϴ��ɹ�", url);
            }
            catch (Exception ex)
            {
                return MessageModel<string>.Fail($"�ļ��ϴ��쳣:{ex.Message}");
            }
        }

        [HttpPost]
        public async Task<MessageModel<string>> upBase64Image(UploadBase64Dto  data)
        {

            try
            {

                if (!(UploadInfo.uploadName.Equals(data.uploadName) && UploadInfo.uploadPass.Equals(data.uploadPass)))
                    return MessageModel<string>.Fail("�˺�������֤����"); 

                string year = DateTime.Now.ToString("yyyy");
                string date = DateTime.Now.ToString("yyyyMMdd");
                string dic = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", year, date);
                if (!Directory.Exists(dic))
                    Directory.CreateDirectory(dic);

                var suffix = ".png";
                var name = $"{date}_{StringHelper.GetGUID()}{suffix}";

                if (!UploadInfo.allowTypes.Contains(suffix))
                {
                    return MessageModel<string>.Fail($"�ļ����Ͳ�֧��:{suffix}");
                }

                var path = Path.Combine(dic, name);

                string base64Data = data.base64Image.Substring(data.base64Image.IndexOf(",") + 1);

                byte[] imageBytes = Convert.FromBase64String(base64Data); 

                await System.IO.File.WriteAllBytesAsync(path, imageBytes);

                var url = $"{UploadInfo.uploadUrl}/{year}/{date}/{name}";


                return MessageModel<string>.Success("�ļ��ϴ��ɹ�", url);
            }
            catch (Exception ex)
            {
                return MessageModel<string>.Fail($"�ļ��ϴ��쳣:{ex.Message}");
            }



        }


    }
}