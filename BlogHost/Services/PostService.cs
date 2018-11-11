using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BlogHost.Models;
using BlogHost.ServiceInterfaces;
using BlogHost.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;

namespace BlogHost.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostService(UserManager<ApplicationUser> userManager, IPostRepository postRepository, IBlogRepository blogRepository)
        {
            _postRepository = postRepository;
            _blogRepository = blogRepository;
            _userManager = userManager;
        }

        private ApplicationUser GetUser(ClaimsPrincipal currentUser)
        {
            return _userManager.GetUserAsync(currentUser).Result;
        }

        private bool HasAccess(int? postId, ClaimsPrincipal currentUser)
        {
            Post post = _postRepository.GetPost(postId);

            var user = GetUser(currentUser);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            return (user.Id == post.Author.Id || userRoles.Contains("admin") || userRoles.Contains("moderator"));
        }

        public void Delete(int? id, ClaimsPrincipal currentUser)
        {
            if (HasAccess(id, currentUser))
            {
                _postRepository.Delete(id);
            }
        }

        public Post GetPost(int? id, ClaimsPrincipal currentUser)
        {
            if (!HasAccess(id, currentUser))
            {
                return null;
            }
            return _postRepository.GetPost(id);
        }

        public void Edit(Post post)
        {
            Post databasePost = _postRepository.GetPost(post.Id);
            databasePost.Title = post.Title;
            databasePost.Text = post.Text;
            databasePost.LastUpdated = DateTime.Now;
            _postRepository.Update(databasePost);
        }

        public void Create(Post post, ClaimsPrincipal currentUser, int blogId)
        {
            post.Blog = _blogRepository.GetBlog(blogId);
            post.Author = GetUser(currentUser);
            post.Created = post.LastUpdated = DateTime.Now;
            _postRepository.Create(post);
        }

        public IEnumerable<Post> GetPagePosts(int page, int pageSize, int blogId, out int postsCount)
        {
            IEnumerable<Post> posts = _postRepository.GetPostList(blogId);
            postsCount = posts.Count();
            IEnumerable<Post> postsPerPage = posts.Skip((page - 1) * pageSize).Take(pageSize);

            return postsPerPage;
        }
    }
}
