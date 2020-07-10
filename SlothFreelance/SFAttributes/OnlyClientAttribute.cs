using SlothFreelance.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlothFreelance.SFAttributes
{
    public class OnlyClientAttribute : FilterAttribute, IActionFilter
    {
        private UnitOfWork unitOfWork;

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            unitOfWork = new UnitOfWork();
            var user = unitOfWork.Users.GetItemById(int.Parse(filterContext.HttpContext.User.Identity.Name));

            if (user.RoleId != 2)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                    { "controller", "Account" }, { "action", "OnlyClient" }, {"area", ""}
                   });
            }
        }
    }
}