using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using BlogHost.Models;

namespace BlogHost.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }
        public string PageAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination pagination-lg");

            if (PageModel.NeedsFirstPage)
            {
                TagBuilder firstItem = CreateTag(1, urlHelper);
                tag.InnerHtml.AppendHtml(firstItem);
            }

            if (PageModel.NeedsBeginingElipsis)
            {
                TagBuilder ellipsisItem = CreateTag(PageModel.TotalPages, urlHelper, "...", true);
                tag.InnerHtml.AppendHtml(ellipsisItem);
            }

            if (PageModel.HasPreviousPage)
            {
                TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);
            tag.InnerHtml.AppendHtml(currentItem);

            if (PageModel.HasNextPage)
            {
                TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
                tag.InnerHtml.AppendHtml(nextItem);
            }

            if (PageModel.NeedsEndingElipsis)
            {
                TagBuilder ellipsisItem = CreateTag(PageModel.TotalPages, urlHelper, "...", true);
                tag.InnerHtml.AppendHtml(ellipsisItem);
            }

            if (PageModel.NeedsLastPage)
            {
                TagBuilder lastItem = CreateTag(PageModel.TotalPages, urlHelper);
                tag.InnerHtml.AppendHtml(lastItem);
            }

            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper, string symbol = "", bool disableFlag = false)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");

            if (disableFlag)
            {
                item.AddCssClass("disabled");
            }
            else if (pageNumber == this.PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new { page = pageNumber });
            }

            if (!symbol.Equals(""))
            {
                link.InnerHtml.Append(symbol);
            }
            else
            {
                link.InnerHtml.Append(pageNumber.ToString());
            }
            item.InnerHtml.AppendHtml(link);

            return item;
        }
    }
}