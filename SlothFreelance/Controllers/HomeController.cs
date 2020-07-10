using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SlothFreelance.Unit;

namespace SlothFreelance.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork;
        public HomeController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            var lastTasks = unitOfWork.Tasks.GetTasksCount(3);

            return View(lastTasks);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [ChildActionOnly]
        [Authorize]
        public ActionResult HeaderUser()
        {
            HeaderViewModel headerViewModel = new HeaderViewModel { User = unitOfWork.Users.GetUserByIdWithJoin(int.Parse(User.Identity.Name), true) };
            return PartialView("_HeaderUser", headerViewModel);
        }
    }
}