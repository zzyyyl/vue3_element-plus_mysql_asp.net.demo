namespace WebApi.Tables
{
    public class ProjectUndertaken
    {
        public string Tid { get; set; } = null!;
        public string Jid { get; set; } = null!;
        public int? Jtrank { get; set; }
        public float? Jtbudget { get; set; }
    }
}
