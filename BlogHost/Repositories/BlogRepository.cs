using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogHost.Data;
using BlogHost.Models;
using BlogHost.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogHost.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Blog> GetBlogList(ApplicationUser user)
        {
            return _context.Blogs
                .Include(author => author.Author)
                .Where(author => author.Author == user);
        }

        public void Create(Blog blog)
        {
            _context.Blogs.Add(blog);
            Save();
        }

        public void Delete(int? id)
        {
            var blog = GetBlog(id);
            if (blog == null) return;

            _context.Blogs.Remove(blog);
            Save();
        }

        public void Update(Blog blog)
        {
            _context.Update(blog);
            Save();
        }

        public Blog GetBlog(int? id)
        {
            var blog = _context.Blogs
                .Include(element => element.Author)
                .FirstOrDefault(element => element.Id == id);
            return blog;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
