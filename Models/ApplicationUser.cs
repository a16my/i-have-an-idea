using Microsoft.AspNetCore.Identity;

namespace BirFikrimVar.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;

        // Navigation collections
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}