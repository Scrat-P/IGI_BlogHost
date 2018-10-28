using System;
using System.Collections.Generic;
using System.Linq;
using BlogHost.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogHost.Components
{
    public class PopularTags : ViewComponent
    {
        List<Tuple<string, int>> tags = new List<Tuple<string, int>>();

        public PopularTags(ApplicationDbContext context)
        {
            var allPosts = context.Tags.Include(element => element.Posts);
            foreach (var element in allPosts)
            {
                tags.Add(new Tuple<string, int>(element.Name, element.Posts.Count()));
            }
        }

        public IViewComponentResult Invoke()
        {
            var sortedDict = from entry in tags orderby entry.Item2 ascending select entry;
            ViewBag.PopularTags = sortedDict.TakeLast(10);

            return View();
        }

    }
}
