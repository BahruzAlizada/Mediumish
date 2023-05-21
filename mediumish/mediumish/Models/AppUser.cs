using Microsoft.AspNetCore.Identity;

namespace mediumish.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsDeactive { get; set; }
    }
}
