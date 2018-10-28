using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogHost.Data;
using BlogHost.Models;
using BlogHost.Models.BlogViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogHost.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public BlogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            List<Blog> blogs = _context.Blogs.Include(author => author.Author).Where(author => author.Author == user).ToList();

            IEnumerable<Blog> blogsPerPage = blogs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(blogs.Count(), page, pageSize);
            IndexViewModel<Blog> viewModel = new IndexViewModel<Blog>
            {
                PageViewModel = pageViewModel,
                Items = blogsPerPage
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateBlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Blog blog = new Blog()
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description
                };

                blog.Author = await _userManager.GetUserAsync(User);
                _context.Blogs.Add(blog);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(viewModel);
        }

        public IActionResult Show(int? id, int page = 1, int pageSize = 9)
        {
            if (!_context.Blogs.Any(element => element.Id == id))
            {
                return NotFound();
            }

            IEnumerable<Post> posts = _context.Posts
                .Include(element => element.Author)
                .Include(element => element.Blog)
                .Where(element => element.Blog.Id == id);
            IEnumerable<Post> postsPerPage = posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            PageViewModel pageViewModel = new PageViewModel(posts.Count(), page, pageSize);
            IndexViewModel<Post> viewModel = new IndexViewModel<Post>
            {
                PageViewModel = pageViewModel,
                Items = postsPerPage
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            Blog blog = _context.Blogs.Include(user => user.Author).FirstOrDefault(element => element.Id == id);

            if (blog == null)
            {
                return NotFound();
            }
            else if (!HasAccess(blog.Author, User))
            {
                return new StatusCodeResult(401);
            }

            EditBlogViewModel viewModel = new EditBlogViewModel()
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditBlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Blog blog = _context.Blogs.FirstOrDefault(element => element.Id == viewModel.Id);

                blog.Title = viewModel.Title;
                blog.Description = viewModel.Description;
                _context.Update(blog);
                await _context.SaveChangesAsync();

                return RedirectToRoute(new { controller = "Blog", action = "Show", id = blog.Id });
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(element => element.Id == id);

            if (blog == null)
            {
                return NotFound();
            }
            else if (!HasAccess(blog.Author, User))
            {
                return new StatusCodeResult(401);
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
