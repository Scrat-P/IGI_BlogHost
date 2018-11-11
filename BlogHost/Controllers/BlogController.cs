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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlogService _blogService;
        private readonly IPostService _postService;

        public BlogController(
            UserManager<ApplicationUser> userManager, 
            IBlogService blogService,
            IPostService postService)
        {
            _userManager = userManager;
            _blogService = blogService;
            _postService = postService;
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
        public IActionResult Create(CreateBlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Blog blog = new Blog()
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description
                };
                _blogService.Create(blog, User);
                
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

            int postsCount;
            IEnumerable<Post> postsPerPage = _postService.GetPagePosts(page, pageSize, (int)id, out postsCount);

            PageViewModel pageViewModel = new PageViewModel(postsCount, page, pageSize);
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
