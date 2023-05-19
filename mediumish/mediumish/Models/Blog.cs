using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mediumish.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name can not be null")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description can not be null")]
        public string Description { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow.AddHours(4);
        public bool IsFavorite { get; set; }
        public bool IsDeactive { get; set; }
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public List<BlogTag> BlogTags { get; set; }
    }
}
