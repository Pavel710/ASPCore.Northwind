using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epam.ASPCore.Northwind.WebUI.Helpers.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "northwind-id")]
    public class LinkTagHelper : TagHelper
    {
        [HtmlAttributeName("northwind-id")]
        public int NorthwindImageId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("href", "/CategoryImages/" + NorthwindImageId);
            output.Attributes.RemoveAll("northwind-id");
        }
    }
}
