using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogHost.Data;
using BlogHost.Models;
using BlogHost.Models.PostViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogHost.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private bool HasAccess(ApplicationUser entityUser, ClaimsPrincipal currentUser)
        {
            var user = _userManager.GetUserAsync(currentUser).Result;
            var userRoles = _userManager.GetRolesAsync(user).Result;

            return (user.Id == entityUser.Id || userRoles.Contains("admin") || userRoles.Contains("moderator"));
        }

        [Authorize]
        public IActionResult Create(int? blogId)
        {
            if (!_context.Blogs.Any(element => element.Id == blogId))
            {
                return NotFound();
            }

            CreatePostViewModel viewModel = new CreatePostViewModel()
            {
                BlogId = (int)blogId
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime nowTime = DateTime.Now;
                Post post = new Post()
                {
                    Title = viewModel.Title,
                    Blog = _context.Blogs.FirstOrDefault(blog => blog.Id == viewModel.BlogId),
                    Author = await _userManager.GetUserAsync(User),
                    Created = nowTime,
                    LastUpdated = nowTime,
                    Text = viewModel.Text
                };
                _context.Posts.Add(post);
                _context.SaveChanges();

                return RedirectToAction("Show", "Blog", new { id = viewModel.BlogId });
            }

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            Post post = _context.Posts.Include(user => user.Author).FirstOrDefault(element => element.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            else if (!HasAccess(post.Author, User))
            {
                return new StatusCodeResult(401);
            }

            EditPostViewModel viewModel = new EditPostViewModel()
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditPostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Post post = _context.Posts.FirstOrDefault(element => element.Id == viewModel.Id);

                post.Title = viewModel.Title;
                post.Text = viewModel.Text;
                post.LastUpdated = DateTime.Now;
                _context.Update(post);
                await _context.SaveChangesAsync();

                return RedirectToAction("Show", "Post", new { id = post.Id });
            }

            return View(viewModel);
        }

        public IActionResult Show(int? id)
        {
            Post post = _context.Posts
                .Include(user => user.Author)
                .Include(blog => blog.Blog)
                .Include(comment => comment.Comments)
                .FirstOrDefault(element => element.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        //[HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            Post post = await _context.Posts
                .Include(user => user.Author)
                .Include(blog => blog.Blog)
                .FirstOrDefaultAsync(element => element.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            else if (!HasAccess(post.Author, User))
            {
                return new StatusCodeResult(401);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Show", "Blog", new { id = post.Blog.Id });
        }

        //public IActionResult PutLike(int? id)
        //{
        //    return View();
        //}

        //public IActionResult CleanLike(int? id)
        //{
        //    return View();
        //}
    }
}
