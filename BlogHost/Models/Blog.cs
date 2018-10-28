using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogHost.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public ApplicationUser Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Post> Posts { get; set; }
    }
}
