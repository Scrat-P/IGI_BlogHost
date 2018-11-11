using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Claims;
using BlogHost.Models;
using BlogHost.ServiceInterfaces;
using BlogHost.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;

namespace BlogHost.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogService(UserManager<ApplicationUser> userManager, IBlogRepository repository)
        {
            _repository = repository;
            _userManager = userManager;
        }

        private ApplicationUser GetUser(ClaimsPrincipal currentUser)
        {
            return _userManager.GetUserAsync(currentUser).Result;
        }

        private bool HasAccess(int? blogId, ClaimsPrincipal currentUser)
        {
            Blog blog = _repository.GetBlog(blogId);

            var user = GetUser(currentUser);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            return (user.Id == blog.Author.Id || userRoles.Contains("admin") || userRoles.Contains("moderator"));
        }

        public void Delete(int? id, ClaimsPrincipal currentUser)
        {
            if (HasAccess(id, currentUser))
            {
                _repository.Delete(id);
            }
        }

        public Blog GetBlog(int? id, ClaimsPrincipal currentUser)
        {
            if (!HasAccess(id, currentUser))
            {
                return null;
            }
            return _repository.GetBlog(id);
        }

        public void Edit(Blog blog)
        {
            Blog databaseBlog = _repository.GetBlog(blog.Id);
            databaseBlog.Title = blog.Title;
            databaseBlog.Description = blog.Description;
            _repository.Update(databaseBlog);
        }

        public void Create(Blog blog)
        {

        }

        public IEnumerable<Blog> GetPageBlogs(int page, int pageSize, ClaimsPrincipal currentUser, out int blogsCount)
        {
            var user = GetUser(currentUser);

            IEnumerable<Blog> blogs = _repository.GetBlogList(user);
            blogsCount = blogs.Count();
            IEnumerable<Blog> blogsPerPage = blogs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return blogsPerPage;
        }
    }
}
