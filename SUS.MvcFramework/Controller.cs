using SUS.HTTP;
using SUS.HTTP.Enums;
using SUS.MvcFramework.ViewEngine;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SUS.MvcFramework
{
    public abstract class Controller
    {
        private SusViewEngine viewEngine;

        public Controller()
        {
            this.viewEngine = new SusViewEngine();
        }

        public HttpRequest Request { get; set; }

        public HttpResponse View(
            object viewModel = null,
            [CallerMemberName]string viewPath = null)
        {
            // Get and parse to HTML global layout
            var layout = System.IO.File.ReadAllText("Views/Shared/_Layout.cshtml");
            layout = layout.Replace("@RenderBody()", "___VIEW_GOES_HERE___");
            layout = this.viewEngine.GetHtml(layout, null);

            // Get and parse to HTML View content
            var viewContent = System.IO.File.ReadAllText(
                "Views/" +
                this.GetType().Name.Replace("Controller", string.Empty) + 
                "/" + viewPath + ".cshtml");
            viewContent = this.viewEngine.GetHtml(viewContent, viewModel);

            // Fill body with parsed HTML
            var responseHtml = layout.Replace("___VIEW_GOES_HERE___", viewContent);

            // Get body bytes
            var responseBodyBytes = Encoding.UTF8.GetBytes(responseHtml);

            // Create new Http Response
            var response = new HttpResponse("text/html", responseBodyBytes);

            return response;
        }

        public HttpResponse File(string path, string contentType)
        {
            var fileBytes = System.IO.File.ReadAllBytes(path);
            var response = new HttpResponse(contentType, fileBytes);

            return response;
        }

        public HttpResponse Redirect(string url)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            response.Headers.Add(new Header("Location", url));

            return response;
        }
    }
}
