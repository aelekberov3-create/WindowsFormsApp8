using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WindowsFormsApp8

{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }

    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }

    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }
    }

    public class AppDbContext : DbContext
    {
        // SQL Server adın fərqlidirsə Server=. hissəsini dəyiş (məsələn: Server=.\SQLEXPRESS)
        public AppDbContext() : base(@"Server=.;Database=StudentGradesDb;Trusted_Connection=True;")
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}