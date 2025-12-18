using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebCoba.Helpers {
    public static class GButtonExtensions {

        public static IHtmlString GButton(
            this HtmlHelper htmlHelper,
            string text,
            string variant = "primary",
            string size = "md",
            string icon = "",
            object htmlAttributes = null
            ) {
            var builder = new TagBuilder("button");

            builder.AddCssClass("g-btn");
            builder.AddCssClass("g-btn-" + variant);
            builder.AddCssClass("g-btn-" + size);

            if (htmlAttributes != null) {
                var attributes = new RouteValueDictionary(htmlAttributes);
                builder.MergeAttributes(attributes);
            }

            string iconHtml = !string.IsNullOrEmpty(icon) ? $"<i class='{icon}'></i> " : "";
            builder.InnerHtml = iconHtml + text;

            return new HtmlString(builder.ToString());
        }
    }
}