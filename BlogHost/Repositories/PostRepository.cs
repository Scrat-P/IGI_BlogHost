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
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Post> GetPostList(int blogId)
        {
            return _context.Posts
                .Include(element => element.Author)
                .Include(element => element.Blog)
                .Include(element => element.Likes)
                .Include(element => element.Comments)
                .Where(element => element.Blog.Id == id);
        }

        public void Create(Post post)
        {
            _context.Posts.Add(post);
            Save();
        }

        public void Delete(int? id)
        {
            var post = GetPost(id);
            if (post == null) return;

            _context.Posts.Remove(post);
            Save();
        }

        public void Update(Post post)
        {
            _context.Update(post);
            Save();
        }

        public Post GetPost(int? id)
        {
            var post = _context.Posts
                .Include(element => element.Author)
                .FirstOrDefault(element => element.Id == id);
            return post;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
