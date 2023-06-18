namespace WebApi.Tables
{
    public class TeacherDetailProject
    {
        public ProjectUndertaken ProjectUndertaken = new();
        public Project Project = new();
        public TeacherDetailProject(ProjectUndertaken projectUndertaken, Project project)
        {
            ProjectUndertaken = projectUndertaken;
            Project = project;
        }
    }
    public class TeacherDetailCourse
    {
        public CourseTaught CourseTaught = new();
        public Course Course = new();
        public TeacherDetailCourse(CourseTaught courseTaught, Course course)
        {
            CourseTaught = courseTaught;
            Course = course;
        }
    }
    public class TeacherDetailPaper
    {
        public PublishPaper PublishPaper = new();
        public Paper Paper = new();
        public TeacherDetailPaper(PublishPaper publishPaper, Paper paper)
        {
            PublishPaper = publishPaper;
            Paper = paper;
        }
    }
    public class TeacherDetail
    {
        public Teacher Teacher;
        public List<TeacherDetailProject> Projects { get; set; } = new() { };
        public List<TeacherDetailCourse> Courses { get; set; } = new() { };
        public List<TeacherDetailPaper> Papers { get; set; } = new() { };
        public TeacherDetail(
            Teacher teacher,
            List<TeacherDetailProject> projects,
            List<TeacherDetailCourse> courses,
            List<TeacherDetailPaper> papers)
        {
            Teacher = teacher;
            Projects = projects;
            Courses = courses;
            Papers = papers;
        }
    }
}
