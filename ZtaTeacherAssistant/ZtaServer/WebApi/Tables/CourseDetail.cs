namespace WebApi.Tables
{
    public class CourseDetail
    {
        public Course Course;
        public List<CourseTaught> Teachers { get; set; } = new List<CourseTaught> { };
        public CourseDetail(Course course, List<CourseTaught> teachers)
        {
            Course = course;
            Teachers = teachers;
        }
    }
}
