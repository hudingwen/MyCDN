namespace netcore.Dto
{
    public class CleanFileDto
    {
        /// <summary>
        /// 本地路径
        /// </summary>
        public string filePath { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 网络路径
        /// </summary>
        public string netPath { get; set; }
    }
}
