namespace netcore.Dto
{
    public class EditorDto
    {
        public int errno { get; set; }
        public string message { get; set; }
        public EditorDtoData data { get; set; } = new EditorDtoData();
    }
    public class EditorDtoData
    {
        public string url { get; set; }
        public string alt { get; set; }
        public string href { get; set; }
        public string poster { get; set; }
    }
}
