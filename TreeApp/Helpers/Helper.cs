using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TreeApp.Models;

namespace TreeApp.Helpers
{
    public static class Helper
    {
        public static MvcHtmlString NoEncodeActionLink(this HtmlHelper htmlHelper,
                                             string text, string title, string action,
                                             string controller,
                                             object routeValues = null,
                                             object htmlAttributes = null)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var builder = new TagBuilder("a") { InnerHtml = text };
            builder.Attributes["title"] = title;
            builder.Attributes["href"] = urlHelper.Action(action, controller, routeValues);
            builder.MergeAttributes(new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));

            return MvcHtmlString.Create(builder.ToString());
        }

        public static string Multiply(this string source, int multiplier)
        {
            var sb = new StringBuilder(multiplier * source.Length);
            for (var i = 0; i < multiplier; i++)
            {
                sb.Append(source);
            }
            return sb.ToString();
        }
    }
}