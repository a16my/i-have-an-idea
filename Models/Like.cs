namespace BirFikrimVar.Models
{
    public class Like
    {
        public int Id { get; set; }

        // Foreign key to Post
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        // Foreign key to ApplicationUser
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}