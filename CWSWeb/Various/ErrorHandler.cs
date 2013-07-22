using CWSWeb.Models;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Various
{
    public class ErrorHandler : IErrorHandler
    {
        private IViewFactory factory;

        public ErrorHandler(IViewFactory factory)
        {
            this.factory = factory;
        }

        public bool HandlesStatusCode(HttpStatusCode code, NancyContext ctx)
        {
            return code == HttpStatusCode.NotFound
                    || code == HttpStatusCode.Forbidden
                    || code == HttpStatusCode.Unauthorized
                    || code == HttpStatusCode.InternalServerError;

        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            IViewRenderer renderer = new DefaultViewRenderer(factory);
            Response response;

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    response = renderer.RenderView(context, "Error/error.cshtml", new Error { ErrorCode = "404", ErrorMessage = "The requested page was not found." });
                    break;
                case HttpStatusCode.Forbidden:
                    response = renderer.RenderView(context, "Error/error.cshtml", new Error { ErrorCode = "403", ErrorMessage = "You don't have access to this page." });
                    break;
                case HttpStatusCode.Unauthorized:
                    response = renderer.RenderView(context, "Error/error.cshtml", new Error { ErrorCode = "401", ErrorMessage = "You are not allowed to view this page." });
                    break;
                case HttpStatusCode.InternalServerError:
                    response = renderer.RenderView(context, "Error/error.cshtml", new Error { ErrorCode = "500", ErrorMessage = "An internal error occured." });

                    object exceptionKey;
                    object exceptionObject;
                    context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out exceptionObject);
                    context.Items.TryGetValue(NancyEngine.ERROR_KEY, out exceptionKey);

                    if (exceptionKey != null)
                        Console.WriteLine(exceptionKey.ToString());

                    if (exceptionObject != null)
                        Console.WriteLine(exceptionObject.ToString());

                    break;
                default:
                    response = renderer.RenderView(context, "Error/error.cshtml", new Error { ErrorCode = ":(", ErrorMessage = "How did I end up here?" });
                    break;
            }

            response.StatusCode = statusCode;
            context.Response = response;
        }
    }
}
