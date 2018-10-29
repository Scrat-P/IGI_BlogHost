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
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            IEnumerable<Post> posts = _context.Posts
                .Include(element => element.Author)
                .Include(element => element.Blog)
                .Include(element => element.Likes)
                .Include(element => element.Comments)
                .Where(element => element.Blog.Id == 1);

            return View(posts);
        }
    }
}
