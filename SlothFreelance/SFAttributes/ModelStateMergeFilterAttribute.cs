using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlothFreelance.SFAttributes
{
    public class ModelStateMergeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller.TempData["ModelState"] == null && filterContext.Controller.ViewData.ModelState != null)
            {
                filterContext.Controller.TempData["ModelState"] = filterContext.Controller.ViewData.ModelState;
            }
            else if (filterContext.Controller.TempData["ModelState"] != null && !filterContext.Controller.ViewData.ModelState.Equals(filterContext.Controller.TempData["ModelState"]))
            {
                filterContext.Controller.ViewData.ModelState.Merge((ModelStateDictionary)filterContext.Controller.TempData["ModelState"]);
            }
            base.OnActionExecuted(filterContext);
        }
    }
}