using SlothFreelance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using SlothFreelance.SFAttributes;
using SlothFreelance.Unit;
using SlothFreelance.AccountController;

namespace SlothFreelance.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private UnitOfWork unitOfWork;
        private readonly FileManager fileManager;
        public SettingsController()
        {
            unitOfWork = new UnitOfWork();
            fileManager = new FileManager();
        }

        [ModelStateMergeFilter]
        public ActionResult Index()
        {
            var roles = unitOfWork.Roles.GetAll();
            Settings settings = SettingsModelInit();
            ViewBag.RedirectUrl = Url.Action("Index", "Settings");
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", settings.RoleId);

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Settings settingsModel)
        {
            var roles = unitOfWork.Roles.GetAll();

            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", settingsModel.RoleId);
            ViewBag.RedirectUrl = Url.Action("Index", "Settings");

            if (ModelState.IsValid)
            {
                if (settingsModel.UserId != int.Parse(User.Identity.Name))
                {
                    settingsModel.UserId = int.Parse(User.Identity.Name);
                }
                var emailInfo = unitOfWork.Users.GetUserByLoginData(settingsModel.Email);

                var checkTasks = unitOfWork.Users.GetUserByIdWithJoin(settingsModel.UserId, false, true, false);
                var checkRequests = unitOfWork.Users.GetUserByIdWithJoinRequests(settingsModel.UserId);

                if (emailInfo != null && checkTasks.Email != settingsModel.Email)
                {
                    ModelState.AddModelError("", "Эта почта уже используется");
                    return View(settingsModel);
                }
                if (checkTasks.Tasks.Count() > 0 && checkTasks.RoleId == 2 && settingsModel.RoleId != 2)
                {
                    ModelState.AddModelError("", "Вы не можете сменить роль, так как у вас есть незакрытая задача");
                    return View(settingsModel);
                }
                else if (checkRequests.TaskRequests.Count() > 0 && checkTasks.RoleId == 1 && settingsModel.RoleId != 1)
                {
                    ModelState.AddModelError("", "Вы не можете сменить роль, так как у вас есть заявка на задачу");
                    return View(settingsModel);
                }

                Users user = UsersModelInit(settingsModel);

                unitOfWork = new UnitOfWork();
                unitOfWork.Users.UpdateItem(user);
                unitOfWork.Save();
            }

            

            return View(settingsModel);
        }

        [HttpPost]
        public ActionResult AddProfileImage(ImageModel imageModel, HttpPostedFileBase uploadImage, string redirectUrl)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                int userId = int.Parse(User.Identity.Name);
                string path = Server.MapPath("~/Content/Assets/Users/") + userId;
                string result = fileManager.AddImage(uploadImage, path, userId, new ImageSize { width = 264, height = 264 });

                if (result == null)
                {
                    var user = unitOfWork.Users.GetItemById(userId);
                    user.Image = userId + "/" + Path.GetFileName(uploadImage.FileName);
                    unitOfWork.Users.UpdateItem(user);
                    unitOfWork.Save();
                }
                else
                {
                    TempData["ImageError"] = result;
                }
            }
            return RedirectToLocal(redirectUrl);
        }
        
        public ActionResult RemoveProfileImage()
        {
            int userId = int.Parse(User.Identity.Name);
            var user = unitOfWork.Users.GetItemById(userId);
            string filePath = null;
            if (user.Image != null)
            {
                filePath = user.Image;
                user.Image = null;
                unitOfWork.Users.UpdateItem(user);
                unitOfWork.Save();

                fileManager.RemoveImage(Server.MapPath("~/Content/Assets/Users/" + "/" + filePath));
            }

            return RedirectToAction("Index", "Settings");
        }

        private Settings SettingsModelInit()
        {
            var user = unitOfWork.Users.GetUserByIdWithJoin(int.Parse(User.Identity.Name), true);

            Settings settings = new Settings
            {
                UserId = user.UserId,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Password = user.Password,
                PasswordAgain = user.Password,
                RoleId = user.RoleId,
                Image = user.Image,
                Country = user.Country,
                City = user.City
            };

            return settings;
        }

        private Users UsersModelInit(Settings settingsModel)
        {
            var user = unitOfWork.Users.GetUserByIdWithJoin(settingsModel.UserId, true);
            settingsModel.Image = user.Image;

            if (settingsModel.Password == null)
            {
                settingsModel.Password = user.Password;
            }

            Users userModel = new Users
            {
                UserId = settingsModel.UserId,
                UserName = settingsModel.UserName,
                Country = settingsModel.Country,
                City = settingsModel.City,
                PhoneNumber = settingsModel.PhoneNumber,
                Email = settingsModel.Email,
                Password = settingsModel.Password,
                Image = settingsModel.Image,
                RoleId = settingsModel.RoleId,
                Money = user.Money
            };

            unitOfWork.Dispose();

            return userModel;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}