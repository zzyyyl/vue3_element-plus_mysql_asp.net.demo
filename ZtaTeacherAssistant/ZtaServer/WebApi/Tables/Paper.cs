namespace WebApi.Tables
{
    public class Paper
    {
        public int Pid { get; set; }
        public string? Pname { get; set; }
        public string? Psource { get; set; }
        public string? Pyear { get; set; }
        public int? Ptype { get; set; }
        public int? Level { get; set; }
    }
}