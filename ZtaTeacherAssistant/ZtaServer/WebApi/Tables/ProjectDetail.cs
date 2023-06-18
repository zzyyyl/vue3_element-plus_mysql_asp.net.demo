namespace WebApi.Tables
{
    public class ProjectDetail
    {
        public Project Project;
        public List<ProjectUndertaken> Managers { get; set; } = new List<ProjectUndertaken> { };
        public ProjectDetail(Project project, List<ProjectUndertaken> managers)
        {
            Project = project;
            Managers = managers;
        }
    }
}
