namespace WebApi.Tables
{
    public class Project
    {
        public string Jid { get; set; } = null!;
        public string? Jname { get; set; }
        public string? Jsource { get; set; }
        public int? Jtype { get; set; }
        public float? Jbudgets { get; set; }
        public int? Styear { get; set; }
        public int? Edyear { get; set; }
    }
}
