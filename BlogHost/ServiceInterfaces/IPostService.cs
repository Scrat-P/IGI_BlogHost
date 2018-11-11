using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BlogHost.Models;

namespace BlogHost.ServiceInterfaces
{
    public interface IPostService
    {
        void Delete(int? id, ClaimsPrincipal currentUser);
        Post GetPost(int? id, ClaimsPrincipal currentUser);
        void Edit(Post post);
        void Create(Post post, ClaimsPrincipal currentUser, int blogId);
        IEnumerable<Post> GetPagePosts(int page, int pageSize, int blogId, out int postsCount);
    }
}
