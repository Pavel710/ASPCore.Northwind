using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epam.ASPCore.Northwind.WebUI.Helpers
{
    public static class LinkExtensions
    {
        public static IHtmlContent NorthwindImageLink<T>(this IHtmlHelper<T> helper, int imageId, string linkText)
        {
            return helper.ActionLink(linkText, imageId.ToString(), "CategoryImages");
        }
    }
}
