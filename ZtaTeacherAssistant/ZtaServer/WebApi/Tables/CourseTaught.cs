namespace WebApi.Tables
{
    public class CourseTaught
    {
        public string Tid { get; set; } = null!;
        public string Cid { get; set; } = null!;
        public int? Tyear { get; set; }
        public int? Tterm { get; set; }
        public int? Thour { get; set; }
    }
}
