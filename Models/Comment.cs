using System;

namespace BirFikrimVar.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Post
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        // Foreign key to ApplicationUser
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}