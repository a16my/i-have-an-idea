using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BirFikrimVar.Data;
using BirFikrimVar.Models;

namespace BirFikrimVar.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var posts = await _context.Posts
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .Include(p => p.OriginalPost).ThenInclude(o => o.User) // show shared source
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.UserFullName = user.FullName;
            return View(posts);
        }

        // GET: /Profile/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // Only allow editing if it’s OWN post & not shared
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id && p.OriginalPostId == null);

            if (post == null)
                return RedirectToAction(nameof(Index)); // safer than AccessDenied → back to profile

            return View(post);
        }

        // POST: /Profile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var existingPost = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id && p.OriginalPostId == null);

            if (existingPost == null)
                return RedirectToAction(nameof(Index));

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;

            _context.Update(existingPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Profile/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);
            if (post == null) return RedirectToAction(nameof(Index));

            return View(post);
        }

        // POST: /Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);
            if (post == null) return RedirectToAction(nameof(Index));

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}