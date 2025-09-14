using System;

namespace BirFikrimVar.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to ApplicationUser
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        // Navigation collections
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();

        // Sharing
        public int? OriginalPostId { get; set; }
        public Post? OriginalPost { get; set; }
    }
}