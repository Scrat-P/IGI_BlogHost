using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogHost.Data;
using BlogHost.Models;
using BlogHost.ServiceInterfaces;
using BlogHost.Models.PostViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogHost.Controllers
{
    public class PostController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlogService _blogService;
        private readonly IPostService _postService;

        public PostController(
            UserManager<ApplicationUser> userManager,
            IBlogService blogService,
            IPostService postService)
        {
            _userManager = userManager;
            _blogService = blogService;
            _postService = postService;
        }

        public IActionResult Create(int? blogId)
        {
            if (_blogService.GetBlog(blogId, User) == null)
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreatePostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post()
                {
                    Title = viewModel.Title,
                    Text = viewModel.Text
                };
                _postService.Create(post, User, viewModel.BlogId);

                return RedirectToAction("Show", "Blog", new { id = viewModel.BlogId });
            }

            return View(viewModel);
        }

        public IActionResult Edit(int? id)
        {
            Post post = _postService.GetPost(id, User);
            if (post == null)
            {
                return NotFound();
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditPostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post()
                {
                    Id = viewModel.Id,
                    Title = viewModel.Title,
                    Text = viewModel.Text
                };
                _postService.Edit(post);

                return RedirectToAction("Show", "Post", new { id = post.Id });
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Show(int? id)
        {
            Post post = _postService.GetPost(id, User);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            int? blogId = _postService.GetPost(id, User)?.Blog.Id;
            _postService.Delete(id, User);

            if (blogId == null)
            {
                return NotFound();
            }
            return RedirectToAction("Show", "Blog", new { id = blogId });
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
