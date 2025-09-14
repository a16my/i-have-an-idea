using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BirFikrimVar.Data;
using BirFikrimVar.Models;

namespace BirFikrimVar.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // FEED
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .Include(p => p.OriginalPost).ThenInclude(o => o.User) // ✅ include shared post
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        // CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "⚠️ Title and content are required!";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var post = new Post
            {
                Title = title.Trim(),
                Content = content.Trim(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // SHARE
        [HttpPost]
        public async Task<IActionResult> Share(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var original = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (original == null) return NotFound();

            var sharedPost = new Post
            {
                Title = original.Title,
                Content = original.Content,
                UserId = user.Id,
                OriginalPostId = original.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(sharedPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // LIKE
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();

            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == id && l.UserId == user.Id);

            if (existingLike == null)
            {
                _context.Likes.Add(new Like { PostId = id, UserId = user.Id });
            }
            else
            {
                _context.Likes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // COMMENT
        [HttpPost]
        public async Task<IActionResult> Comment(int postId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "⚠️ Comment cannot be empty!";
                return RedirectToAction(nameof(Index));
            }

            var comment = new Comment
            {
                PostId = postId,
                UserId = user.Id,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}