namespace WebApi.Tables
{
    public class Course
    {
        public string Cid { get; set; } = null!;
        public string? Cname { get; set; }
        public int? Chour { get; set; }
        public int? Cnature { get; set; }
    }
}
