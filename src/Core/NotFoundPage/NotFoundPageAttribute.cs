﻿using System.Reflection;
using System.Web.Mvc;
using EPiServer.Editor;
using EPiServer.Logging;

namespace Knowit.NotFound.Core.NotFoundPage
{
    public class NotFoundPageAttribute : ActionFilterAttribute
    {
        private static readonly ILogger Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!PageEditing.PageIsInEditMode)
            {
                Log.Debug("Starting 404 handler action filter");
                var request = filterContext.HttpContext.Request;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                int statusCode = NotFoundPageUtil.GetStatusCode(request);
                filterContext.HttpContext.Response.StatusCode = statusCode;
                filterContext.HttpContext.Response.Status = NotFoundPageUtil.GetStatus(statusCode);
                NotFoundPageUtil.SetCurrentLanguage(filterContext.HttpContext);
                filterContext.Controller.ViewBag.Referrer = NotFoundPageUtil.GetReferer(request);
                filterContext.Controller.ViewBag.NotFoundUrl = NotFoundPageUtil.GetUrlNotFound(request);
                filterContext.Controller.ViewBag.StatusCode = statusCode;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
    
        }
    }
}