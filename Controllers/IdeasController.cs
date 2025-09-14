using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BirFikrimVar.Data;
using BirFikrimVar.Models;

namespace BirFikrimVar.Controllers
{
    [Authorize]
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdeasController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Ideas
        public async Task<IActionResult> Index()
        {
            var ideas = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(ideas);
        }

        // GET: /Ideas/Create
        public IActionResult Create()
        {
            return View(new Post());
        }

        // POST: /Ideas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] Post post)
        {
            if (!ModelState.IsValid) return View(post);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            post.UserId = user.Id;
            post.CreatedAt = DateTime.UtcNow;

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Your idea was submitted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Ideas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var idea = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (idea == null) return NotFound();

            return View(idea);
        }
    }
}