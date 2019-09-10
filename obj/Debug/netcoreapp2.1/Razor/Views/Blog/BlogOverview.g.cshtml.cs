#pragma checksum "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e716ac2a86b57b822ed94a8d1bbbcb389f3589b9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Blog_BlogOverview), @"mvc.1.0.view", @"/Views/Blog/BlogOverview.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Blog/BlogOverview.cshtml", typeof(AspNetCore.Views_Blog_BlogOverview))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "/home/taco/RiderProjects/tayko.co/Views/_ViewImports.cshtml"
using Tayko.co;

#line default
#line hidden
#line 2 "/home/taco/RiderProjects/tayko.co/Views/_ViewImports.cshtml"
using Tayko.co.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e716ac2a86b57b822ed94a8d1bbbcb389f3589b9", @"/Views/Blog/BlogOverview.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1a8d5be8871072a23922de62c066ec1e7d3762e3", @"/Views/_ViewImports.cshtml")]
    public class Views_Blog_BlogOverview : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<BlogDataManager>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Blog", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "LoadBlog", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
  
    ViewData["Title"] = "Blog";

#line default
#line hidden
            BeginContext(60, 61, true);
            WriteLiteral("\n<h1>Articles below</h1>\n<hr/>\n<ul class=\"article-overview\">\n");
            EndContext();
#line 9 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
      
        foreach (var article in Model.Articles)
        {

#line default
#line hidden
            BeginContext(186, 33, true);
            WriteLiteral("            <li>\n                ");
            EndContext();
            BeginContext(219, 889, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "eea00e0265f5447a9b65bb86b10b667c", async() => {
                BeginContext(300, 22, true);
                WriteLiteral("\n                    \n");
                EndContext();
#line 15 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                     if (article.Date != DateTime.MinValue)
                    {

#line default
#line hidden
                BeginContext(404, 77, true);
                WriteLiteral("                        <p class=\"article-date\">\n                            ");
                EndContext();
                BeginContext(482, 26, false);
#line 18 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                       Write(article.Date.ToString("D"));

#line default
#line hidden
                EndContext();
                BeginContext(508, 30, true);
                WriteLiteral("\n                        </p>\n");
                EndContext();
#line 20 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                    }

#line default
#line hidden
                BeginContext(560, 24, true);
                WriteLiteral("                    <h2>");
                EndContext();
                BeginContext(585, 13, false);
#line 21 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                   Write(article.Title);

#line default
#line hidden
                EndContext();
                BeginContext(598, 7, true);
                WriteLiteral("</h2>\n\n");
                EndContext();
#line 23 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                      
                        var articleDescription = article.Description;
                        if (articleDescription.Length > 250)
                        {
                            articleDescription = articleDescription.Substring(0, 250);
                        }
                    

#line default
#line hidden
                BeginContext(920, 117, true);
                WriteLiteral("                    <div class=\"article-description\">\n                        <div></div>\n                        <p>");
                EndContext();
                BeginContext(1038, 18, false);
#line 32 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                      Write(articleDescription);

#line default
#line hidden
                EndContext();
                BeginContext(1056, 48, true);
                WriteLiteral("</p>\n                    </div>\n                ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-article", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#line 13 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
                                                                      WriteLiteral(article.Name);

#line default
#line hidden
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["article"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-article", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["article"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1108, 19, true);
            WriteLiteral("\n            </li>\n");
            EndContext();
#line 36 "/home/taco/RiderProjects/tayko.co/Views/Blog/BlogOverview.cshtml"
        }
    

#line default
#line hidden
            BeginContext(1143, 5, true);
            WriteLiteral("</ul>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<BlogDataManager> Html { get; private set; }
    }
}
#pragma warning restore 1591
