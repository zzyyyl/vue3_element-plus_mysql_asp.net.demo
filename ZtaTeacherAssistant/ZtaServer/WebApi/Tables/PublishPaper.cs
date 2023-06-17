namespace WebApi.Tables
{
    public class PublishPaper
    {
        public string Tid { get; set; } = null!;
        public int Pid { get; set; }
        public int? Ptrank { get; set; }
        public bool? Correspond { get; set; }
    }
}
