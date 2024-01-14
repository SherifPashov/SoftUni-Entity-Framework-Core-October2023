using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
       
        public int ResourceId { get; set; }
        
        [MaxLength(50)]
        [Unicode]
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public ResourceType ResourseType{ get; set; }
        public int CourseId { get; set; }
        
        public Course Course { get; set; } = null!;
    }
}
