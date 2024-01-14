using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        
        public int HomeworkId { get; set; }

        public string Content { get; set; } = null!;
        public ContentType ContentType { get; set; }
        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        
        public Student Student { get; set; } = null!;
        public int CourseId { get; set; }
        
        public Course Course { get; set; } = null!;
    }
}
