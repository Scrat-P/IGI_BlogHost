using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogHost.Models
{
    public class Post
    {
        public int Id { get; set; }
        public ApplicationUser Author { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Blog Blog { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }

        public List<PostTag> Tags { get; set; }
    }
}
