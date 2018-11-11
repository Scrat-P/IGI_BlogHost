using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BlogHost.Data;
using BlogHost.Models;
using BlogHost.ServiceInterfaces;
using BlogHost.Models.BlogViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogHost.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlogService _blogService;

        public BlogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlogService blogService)
        {
            _context = context;
            _userManager = userManager;
            _blogService = blogService;
        }

        public IActionResult Index(int page = 1, int pageSize = 3)
        {
            int blogsCount;
            var blogsPerPage = _blogService.GetPageBlogs(page, pageSize, User, out blogsCount);

            PageViewModel pageViewModel = new PageViewModel(blogsCount, page, pageSize);
            IndexViewModel<Blog> viewModel = new IndexViewModel<Blog>
            {
                PageViewModel = pageViewModel,
                Items = blogsPerPage
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Blog blog = new Blog()
                {
                    Author = await _userManager.GetUserAsync(User),
                    Title = viewModel.Title,
                    Description = viewModel.Description
                };
                _blogService.Create(blog);
                
                return RedirectToAction("Index");

            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Show(int? id, int page = 1, int pageSize = 9)
        {
            if (_blogService.GetBlog(id, User) == null)
            {
                return NotFound();
            }

            IEnumerable<Post> posts = _context.Posts
                .Include(element => element.Author)
                .Include(element => element.Blog)
                .Include(element => element.Likes)
                .Include(element => element.Comments)
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

        public IActionResult Edit(int? id)
        {
            Blog blog = _blogService.GetBlog(id, User);
            if (blog == null)
            {
                return NotFound();
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditBlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Blog blog = new Blog()
                {
                    Id = viewModel.Id,
                    Title = viewModel.Title,
                    Description = viewModel.Description
                };
                _blogService.Edit(blog);

                return RedirectToRoute(new { controller = "Blog", action = "Show", id = blog.Id });
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            _blogService.Delete(id, User);
            return RedirectToAction("Index");
        }
    }
}
