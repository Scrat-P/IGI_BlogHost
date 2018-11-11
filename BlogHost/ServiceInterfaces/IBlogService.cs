using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BlogHost.Models;

namespace BlogHost.ServiceInterfaces
{
    public interface IBlogService
    {
        void Delete(int? id, ClaimsPrincipal currentUser);
        Blog GetBlog(int? id, ClaimsPrincipal currentUser);
        void Edit(Blog blog);
        void Create(Blog blog, ClaimsPrincipal currentUser);
        IEnumerable<Blog> GetPageBlogs(int page, int pageSize, ClaimsPrincipal currentUser, out int blogsCount);
    }
}
