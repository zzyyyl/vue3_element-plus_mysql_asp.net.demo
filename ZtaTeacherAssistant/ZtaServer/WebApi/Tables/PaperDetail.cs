namespace WebApi.Tables
{
    public class PaperDetail
    {
        public Paper Paper;
        public List<PublishPaper> Authors { get; set; } = new List<PublishPaper> { };
        public PaperDetail(Paper paper, List<PublishPaper> authors)
        {
            Paper = paper;
            Authors = authors;
        }
    }
}
