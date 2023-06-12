namespace WebApi.Tables
{
    /*
   tid                  char(5) not null  comment '',
   tname                char(255)  comment '',
   gender               int  comment '',
   title                int  comment '',
     */
    public class Teacher
    {
        public string? Tid { get; set; }
        public string? Tname { get; set; }
        public int? Gender { get; set; }
        public int? Title { get; set; }
        public override string ToString()
        {
            return $"tid:{Tid},tname:{Tname},gender:{Gender},title:{Title}";
        }
    }
}
