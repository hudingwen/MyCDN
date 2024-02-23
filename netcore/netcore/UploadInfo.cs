namespace netcore
{
    public static class UploadInfo
    {

        public static readonly string uploadUrl = ConfigHelper.GetValue(new string[] { "uploadInfo", "uploadUrl" });
        public static readonly string uploadName = ConfigHelper.GetValue(new string[] { "uploadInfo", "uploadName" });
        public static readonly string uploadPass = ConfigHelper.GetValue(new string[] { "uploadInfo", "uploadPass" });
        public static readonly List<string> allowTypes = ConfigHelper.GetList<string>(new string[] { "uploadInfo", "allowTypes" });
    }
}
