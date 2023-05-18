using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mediumish.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name can not be null")]
        public string Name { get; set; }
        public List<BlogTag> BlogTags { get; set; }
        public bool IsDeactive { get; set; }
    }
}
