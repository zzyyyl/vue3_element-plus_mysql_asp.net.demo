namespace WebApi.Tables
{
    public class TeacherDetail
    {
        public Teacher Teacher;
        public List<ProjectUndertaken> Projects { get; set; } = new List<ProjectUndertaken> { };
        public List<CourseTaught> Courses { get; set; } = new List<CourseTaught> { };
        public List<PublishPaper> Papers { get; set; } = new List<PublishPaper> { };
        public TeacherDetail(Teacher teacher, List<ProjectUndertaken> projects, List<CourseTaught> courses, List<PublishPaper> papers)
        {
            Teacher = teacher;
            Projects = projects;
            Courses = courses;
            Papers = papers;
        }
    }
}
