using SlothFreelance.AccountController;
using SlothFreelance.Mail;
using SlothFreelance.Models;
using SlothFreelance.SFAttributes;
using SlothFreelance.Unit;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlothFreelance.Controllers
{
    public class AccountController : Controller
    {
        private UnitOfWork unitOfWork;
        private AccountControll accountControll;
        private readonly FileManager fileManager;
        public AccountController()
        {
            unitOfWork = new UnitOfWork();
            accountControll = new AccountControll(unitOfWork);
            fileManager = new FileManager();
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.RedirectUrl = Url.Action("Index", "Account");
            ProfileViewModel profileModel = ProfileModelInit(int.Parse(User.Identity.Name));

            return View(profileModel);
        }

        [Authorize]
        public ActionResult OnlyClient()
        {
            return View();
        }

        [Authorize]
        public ActionResult OnlyExecuter()
        {
            return View();
        }

        [Route("User/{id}")]
        public ActionResult UserPage(int? id)
        {
            ProfileViewModel profileModel;

            if (id != null)
            {
                profileModel = ProfileModelInit(id);
                
                if (profileModel == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return HttpNotFound();
            }

            return View("Index", profileModel);
        }

        [OnlyAnonim]
        public ActionResult Login(string returnUrl)
        {
            var roles = unitOfWork.Roles.GetAll();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName");

            if (returnUrl != null)
            {
                ViewBag.ReturnUrl = returnUrl;
            }

            return View();
        }

        [OnlyAnonim]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel, string returnUrl)
        {
            var roles = unitOfWork.Roles.GetAll();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName");

            if (ModelState.IsValid)
            {
                var user = unitOfWork.Users.GetUserByLoginData(loginModel.Email, loginModel.Password);
                if (user != null)
                {
                    accountControll.UserAuth(user.UserId);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                }
            }
            
            return View();
        }

        [OnlyAnonim]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid) 
            {
                var user = unitOfWork.Users.GetUserByLoginData(registerModel.Email);

                if (user == null)
                {
                    var registerStatus = accountControll.UserRegister(registerModel);

                    if (registerStatus)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return RedirectToAction("Login", "Account");
        }

        [OnlyAnonim]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [OnlyAnonim]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword resetPassword)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = unitOfWork.Users.GetUserByLoginData(resetPassword.Email);
                if (checkEmail == null)
                {
                    ModelState.AddModelError("", "Пользователь с таким Email не зарегистрирован");
                    return View();
                }
                Random random = new Random();
                int code = random.Next(1000, 9999);

                MailTools mail = new MailTools();

                var result = mail.SendMailToUser(resetPassword.Email, "Сброс пароля", $"<p>Код для сброса пароля: <b>{ code }</b></p>");

                if (result != null)
                {
                    ModelState.AddModelError("", result);
                    return View();
                }

                HttpCookie cookie = new HttpCookie("code", code.ToString());
                cookie.Expires = DateTime.Now.AddHours(2);

                Response.Cookies.Add(cookie);
                return RedirectToAction("ResetCode", new { email = resetPassword.Email });
            }
            return View();
        }

        [OnlyAnonim]
        public ActionResult ResetCode(string email)
        {
            Models.ResetCode resetCode = new ResetCode { Email = email };
            return View("ResetCode", resetCode);
        }

        [OnlyAnonim]
        [HttpPost]
        public ActionResult ResetCode(ResetCode resetCode)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("code");
                if (resetCode.Code != cookie.Value.ToString())
                {
                    ModelState.AddModelError("", "Код для сброса неверен");
                    return View(resetCode);
                }

                string password = "";
                Random random = new Random();

                while (password.Length < 10)
                {
                    char c = (char)random.Next(33, 125);
                    if (char.IsLetterOrDigit(c))
                    {
                        password += c;
                    }
                }

                MailTools mail = new MailTools();

                var result = mail.SendMailToUser(resetCode.Email, "Сброс пароля", $"<p>Ваш новый пароль: <b>{ password }</b></p>");

                if (result != null)
                {
                    ModelState.AddModelError("", result);
                    return View(resetCode);
                }

                var user = unitOfWork.Users.GetUserByLoginData(resetCode.Email);
                user.Password = password;
                unitOfWork.Dispose();
                unitOfWork = new UnitOfWork();
                unitOfWork.Users.UpdateItem(user);
                unitOfWork.Save();

                ViewBag.SuccessMessage = "Ваш пароль был отправлен на почту";
            }
            return View(resetCode);
        }

        [Authorize]
        public ActionResult Service()
        {
            var services = unitOfWork.Users.GetUserByIdWithJoin(int.Parse(User.Identity.Name), false, false, true).Services.ToList();
            return PartialView("_Service", services);
        }

        [Authorize]
        public ActionResult AddService()
        {
            return PartialView("_AddService");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public int? AddService(AddService addService)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Services.AddNewItem(ServicesModelInit(addService));
                unitOfWork.Save();

                int? ServiceId = unitOfWork.Users.GetUserByIdWithJoin(int.Parse(User.Identity.Name), false, false, true).Services.FirstOrDefault(s => s.ServiceName == addService.ServiceName).ServiceId;
                return ServiceId;
            }

            return null;
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditService(int? id)
        {
            if (id != null)
            {
                var service = unitOfWork.Services.GetItemById(id);
                if (service != null)
                {
                    AddService addService = new AddService { ServiceId = service.ServiceId, ServiceName = service.ServiceName };
                    return PartialView("_EditService", addService);
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public int? EditService(AddService addService)
        {
            if (ModelState.IsValid)
            {
                var service = unitOfWork.Services.GetItemById(addService.ServiceId);
                if (service != null)
                {
                    service.ServiceName = addService.ServiceName;
                    unitOfWork.Services.UpdateItem(service);
                    unitOfWork.Save();

                    return service.ServiceId;
                }
            }

            return null;
        }

        [Authorize]
        public ActionResult EditServiceImage()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditServiceImage(HttpPostedFileBase uploadImage, ServiceImageEdit serviceImageEdit)
        {
            if (ModelState.IsValid)
            {
                if (uploadImage == null)
                {
                    ModelState.AddModelError("", "Ошибка при изменении изображения сервиса");
                    return EditServiceImage();
                }
                int userId = int.Parse(User.Identity.Name);
                string path = Server.MapPath("~/Content/Assets/Users/" + userId);
                string result = fileManager.AddImage(uploadImage, path, userId, new ImageSize { });

                if (result == null)
                {
                    var service = unitOfWork.Services.GetItemById(serviceImageEdit.ServiceId);

                    if (service == null)
                    {
                        ModelState.AddModelError("", "Услуга не найдена");
                        return EditServiceImage();
                    }

                    service.ServiceImage = userId + "/" + Path.GetFileName(uploadImage.FileName);
                    unitOfWork.Services.UpdateItem(service);
                    unitOfWork.Save();
                }
                else
                {
                    ModelState.AddModelError("", result);
                    return EditServiceImage();
                }

                return Service();
            }

            return EditServiceImage();
        }

        [Authorize]
        public string NoMoneyNoHoney()
        {
            return "Недостаточно средств на счету";
        }

        public ActionResult DeleteService(int? id)
        {
            if (id != null)
            {
                var service = unitOfWork.Services.GetItemById(id);
                if (service != null)
                {
                    if (service.UserId != int.Parse(User.Identity.Name))
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return HttpNotFound();
                }
                unitOfWork.Services.DeleteItem(id);
                unitOfWork.Save();
            }
            else
            {
                return HttpNotFound();
            }
            return Service();
        }

        [Authorize]
        public ActionResult MoneyAccount()
        {
            MoneyModel moneyModel = new MoneyModel { Money = 100, UserId = int.Parse(User.Identity.Name) };
            return View(moneyModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult MoneyAccount(MoneyModel moneyModel)
        {
            if (ModelState.IsValid)
            {
                MoneyAccount moneyAccount = new MoneyAccount(unitOfWork);
                moneyAccount.AddMoney(moneyModel.UserId, moneyModel);
                return RedirectToAction("Index", "Account");
            }
            return View();
        }

        [Authorize]
        [OnlyClient]
        public ActionResult MyTasks()
        {
            var tasks = unitOfWork.Users.GetUserByIdWithJoin(int.Parse(User.Identity.Name), false, true);

            for (int i = 0; i < tasks.Tasks.Count(); i++)
            {
                tasks.Tasks.ToList()[i].Category = unitOfWork.Tasks.GetTaskByIdWithJoin(i + 1, true).Category;
            }

            return View(tasks);
        }

        [Authorize]
        [OnlyExecuter]
        public ActionResult MyRequests()
        {
            var requests = unitOfWork.Users.GetUserByIdWithJoinRequests(int.Parse(User.Identity.Name));

            for (int i = 0; i < requests.TaskRequests.Count(); i++)
            {
                requests.TaskRequests.ToList()[i].Task = unitOfWork.TaskRequests.GetRequestByIdWithJoin(i + 1, false, true).Task;
            }

            return View(requests);
        }
        
        [Authorize]
        public ActionResult EditRequest(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var taskRequest = unitOfWork.TaskRequests.GetItemById(id);

            if (taskRequest.UserId != int.Parse(User.Identity.Name) || taskRequest == null)
            {
                return HttpNotFound();
            }

            return View(taskRequest);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditRequest(TaskRequests taskRequests)
        {
            taskRequests.UserId = int.Parse(User.Identity.Name);
            var request = unitOfWork.TaskRequests.GetItemById(taskRequests.RequestId);
            unitOfWork.Dispose();
            if (ModelState.IsValid)
            {
                if (request == null || taskRequests.TaskId != request.TaskId)
                {
                    return HttpNotFound();
                }
                unitOfWork = new UnitOfWork();
                unitOfWork.TaskRequests.UpdateItem(taskRequests);
                unitOfWork.Save();
                return RedirectToAction("MyRequests");
            }
            return View();
        }

        [Authorize]
        public ActionResult DeleteRequest(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var request = unitOfWork.TaskRequests.GetItemById(id);

            if (request.UserId != int.Parse(User.Identity.Name))
            {
                return HttpNotFound();
            }

            unitOfWork.TaskRequests.DeleteItem(id);
            unitOfWork.Save();

            return RedirectToAction("MyRequests");
        }

        [HttpGet]
        public JsonResult CheckEmail(string email)
        {
            var user = unitOfWork.Users.GetUserByLoginData(email);
            var result = true;

            if (user != null)
            {
                result = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckService(string ServiceName)
        {
            var service = unitOfWork.Users.GetUserByIdWithJoin(int.Parse(User.Identity.Name), false, false, true).Services.FirstOrDefault(s => s.ServiceName == ServiceName);
            var result = true;

            if (service != null)
            {
                result = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Logout()
        {
            accountControll.Logout();

            return RedirectToAction("Index", "Home");
        }

        public string Test(int[] filter)
        {
            string filters = "";
            for (int i = 0; i < filter.Length; i++)
            {
                filters += $" {filter[i]}"; 
            }
            return filters;
        }

        private ProfileViewModel ProfileModelInit(int? id)
        {
            if (id != null)
            {
                var user = unitOfWork.Users.GetUserByIdWithJoin(id, true, true, true);
                if (user != null)
                {
                    bool isMyPage = id == int.Parse(User.Identity.Name) ? true : false;
                    ProfileViewModel profileModel = new ProfileViewModel
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Image = user.Image,
                        Role = user.Role,
                        IsMyPage = isMyPage,
                        Services = user.Services/*,
                    Tasks = user.Tasks*/
                    };
                    return profileModel;
                }  
            }
            return null;
        }

        [Authorize]
        private Services ServicesModelInit(AddService service)
        {
            Services services = new Services { ServiceName = service.ServiceName, UserId = int.Parse(User.Identity.Name) };
            return services;
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