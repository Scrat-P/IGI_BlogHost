using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogHost.Models;

namespace BlogHost.RepositoryInterfaces
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetBlogList(ApplicationUser user); 
        Blog GetBlog(int? id); 
        void Create(Blog blog); 
        void Update(Blog blog);
        void Delete(int? id);
        void Save();
    }
}
