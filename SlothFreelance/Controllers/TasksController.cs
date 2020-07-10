using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SlothFreelance.Models;
using SlothFreelance.Unit;
using SlothFreelance.AccountController;
using System.IO;
using SlothFreelance.SFAttributes;
using SlothFreelance.Mail;

namespace SlothFreelance.Controllers
{
    public class TasksController : Controller
    {
        private UnitOfWork unitOfWork;
        private readonly FileManager fileManager;
        public TasksController()
        {
            unitOfWork = new UnitOfWork();
            fileManager = new FileManager();
        }

        public ActionResult Index(int page = 1, int[] filter = null)
        {
            ViewBag.Page = page;
            ViewBag.Filter = filter;
            return View();
        }

        public ActionResult Tasks(int page, int[] filter = null)
        {
            var tasks = unitOfWork.Tasks.GetTasksByFilter(page, filter);
            if (tasks == null || tasks.Count() == 0)
            {
                return PartialView("_TasksError");
            }
            return PartialView("_Tasks", tasks);
        }

        [Authorize]
        public ActionResult TaskDetails(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var task = unitOfWork.Tasks.GetTaskByIdWithJoin(id, true, true, true);

            if (task == null)
            {
                return HttpNotFound();
            }

            var userInfo = unitOfWork.Users.GetItemById(int.Parse(User.Identity.Name));

            if (userInfo.RoleId != 1 && userInfo.UserId != task.UserId)
            {
                return RedirectToAction("OnlyExecuter", "Account");
            }

            Users executer = null;

            for (int i = 0; i < task.Requests.Count(); i++)
            {
                task.Requests.ToList()[i].User = unitOfWork.TaskRequests.GetRequestByIdWithJoin(i + 1, true, false).User;
                if (task.Requests.ToList()[i].Status == "Выбран")
                {
                    executer = task.Requests.ToList()[i].User;
                }
            }

            if (executer != null)
            {
                if (int.Parse(User.Identity.Name) != executer.UserId && int.Parse(User.Identity.Name) != task.UserId)
                {
                    return HttpNotFound();
                }
            }
            

            if (task.WorkOnTasks.Count() != 0)
            {
                for (int i = 0; i < task.WorkOnTasks.Count(); i++)
                {
                    task.WorkOnTasks.ToList()[i].Users = unitOfWork.WorkOnTask.GetWorkByIdWithJoin(i + 1, true, true).Users;
                }
            }

            return View(task);
        }

        [Authorize]
        public ActionResult AcceptWork(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var task = unitOfWork.Tasks.GetTaskByIdWithJoin(id, false, false, true);
            var request = unitOfWork.Tasks.GetSelectedRequest(id);
            var executer = unitOfWork.Users.GetItemById(request.UserId);

            unitOfWork.Dispose();
            unitOfWork = new UnitOfWork();

            if (task == null || task.UserId != int.Parse(User.Identity.Name))
            {
                return HttpNotFound();
            }

            MoneyAccount moneyAccount = new MoneyAccount(unitOfWork);
            var result = moneyAccount.TaskPayment(task.Users, executer, request.Price);

            if (result != null)
            {
                return RedirectToAction("NoMoneyNoHoney", "Account");
            } 

            task.Status = "Закрыта";
            unitOfWork.Tasks.UpdateItem(task);
            unitOfWork.Save();

            MailTools mail = new MailTools();

            mail.SendMailToUser(executer.Email, "Задача закрыта", $"<p>Заказчик принял вашу работу! На ваш счет начислено <b>{request.Price} рублей.</b></p>");

            return RedirectToAction("TaskDetails", "Tasks", new { id = task.TaskId });
        }

        [Authorize]
        [HttpPost]
        public ActionResult NotAcceptWork(WorkOnTask workOnTask)
        {
            if (ModelState.IsValid)
            {
                workOnTask.UserId = int.Parse(User.Identity.Name);
                var task = unitOfWork.Tasks.GetTaskByIdWithJoin(workOnTask.TaskId, false, true);
                unitOfWork.Dispose();
                unitOfWork = new UnitOfWork();

                if (task == null)
                {
                    return HttpNotFound();
                }

                task.Status = "В работе";

                unitOfWork.WorkOnTask.AddNewItem(workOnTask);
                unitOfWork.Tasks.UpdateItem(task);
                unitOfWork.Save();

                Users executer = null;

                for (int i = 0; i < task.Requests.Count(); i++)
                {
                    task.Requests.ToList()[i].User = unitOfWork.TaskRequests.GetRequestByIdWithJoin(i + 1, true, false).User;
                    if (task.Requests.ToList()[i].Status == "Выбран")
                    {
                        executer = task.Requests.ToList()[i].User;
                    }
                }

                if (executer != null)
                {
                    MailTools mail = new MailTools();

                    mail.SendMailToUser(executer.Email, "Задача в доработке", $"<p>Заказчик отправил задачу в доработку</p>");
                }
                
                return RedirectToAction("TaskDetails", new { id = workOnTask.TaskId });
            }
            return HttpNotFound();
        }

        [Authorize]
        public ActionResult WorkTools(int? id)
        {
            if (id == null)
            {
                return null;
            }
            WorkOnTask work = new WorkOnTask { TaskId = id };

            return PartialView("_WorkTools", work);
        }
        
        [Authorize]
        public ActionResult AddRequest(Tasks task)
        {
            var taskRequest = task.Requests.FirstOrDefault(c => c.UserId == int.Parse(User.Identity.Name));
            if (taskRequest != null)
            {
                return null;
            }
            TaskRequests requests = new TaskRequests { TaskId = task.TaskId};
            return PartialView("_AddRequest", requests);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddRequest(TaskRequests taskRequests)
        {
            taskRequests.UserId = int.Parse(User.Identity.Name);
            if (ModelState.IsValid)
            {
                unitOfWork.TaskRequests.AddNewItem(taskRequests);
                unitOfWork.Save();

                var task = unitOfWork.Tasks.GetTaskByIdWithJoin(taskRequests.TaskId, false, false, true, true);
                
                MailTools mail = new MailTools();

                mail.SendMailToUser(task.Users.Email, "На вашу задачу оставили заявку!", $"<p>На вашу задачу оставили заявку! Зайдите на сайт и проверьте ее.</p>");

                return RedirectToLocal("~/Account/MyRequests");
            }
            return PartialView("_AddRequest");
        }

        [Authorize]
        public ActionResult SetExecuter(int? id, int? userId, int? requestId)
        {
            if (id == null || userId == null)
            {
                return HttpNotFound();
            }

            var task = unitOfWork.Tasks.GetTaskByIdWithJoin(id, false, true);
            var user = unitOfWork.Users.GetItemById(userId);
            var request = unitOfWork.TaskRequests.GetRequestByIdWithJoin(requestId, true, false);
            unitOfWork.Dispose();

            if (task.UserId != int.Parse(User.Identity.Name) || task == null || user == null || request == null)
            {
                return HttpNotFound();
            }

            unitOfWork = new UnitOfWork();

            for (int i = 0; i < task.Requests.Count(); i++)
            {
                task.Requests.ToList()[i].Status = "Не выбран";
            }

            task.Status = "В работе";
            unitOfWork.Tasks.UpdateItem(task);

            request.Status = "Выбран";
            unitOfWork.TaskRequests.UpdateItem(request);

            unitOfWork.Save();

            MailTools mail = new MailTools();

            mail.SendMailToUser(request.User.Email, "Вас выбрали исполнителем!", $"<p>Вас выбрали исполнителем! Можете приступать к работе</p>");

            return RedirectToAction("TaskDetails", new { id });
        }

        [Authorize]
        [OnlyClient]
        public ActionResult CreateTask()
        {
            var category = unitOfWork.Category.GetAll();
            ViewBag.CategoryId = new SelectList(category, "CategoryId", "CategoryName");

            return View();
        }

        [Authorize]
        public ActionResult AddWork(int? id)
        {
            if (id == null)
            {
                return null;
            }
            WorkOnTask work = new WorkOnTask { TaskId = id };
            return PartialView("_AddWork", work);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddWork(HttpPostedFileBase uploadFile, WorkOnTask workOnTask)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile == null)
                {
                    ModelState.AddModelError("", "Выберите файл");
                    return PartialView("_AddWork", workOnTask);
                }
                workOnTask.UserId = int.Parse(User.Identity.Name);
                string path = Server.MapPath("~/Content/Assets/Tasks/" + workOnTask.TaskId);
                string result = fileManager.AddFile(uploadFile, path, workOnTask);
                var task = unitOfWork.Tasks.GetTaskByIdWithJoin(workOnTask.TaskId, false, false, true, true);
                unitOfWork.Dispose();
                unitOfWork = new UnitOfWork();
                if (result == null)
                {
                    if (task == null)
                    {
                        ModelState.AddModelError("", "Задача не найдена");
                        return PartialView("_AddWork", workOnTask);
                    }

                    workOnTask.File = workOnTask.TaskId + "/" + Path.GetFileName(uploadFile.FileName);
                    unitOfWork.WorkOnTask.AddNewItem(workOnTask);
                    task.Status = "В проверке";
                    unitOfWork.Tasks.UpdateItem(task);
                    unitOfWork.Save();

                    MailTools mail = new MailTools();

                    mail.SendMailToUser(task.Users.Email, "Исполнитель загрузил решение", $"<p>Исполнитель загрузил решение. Проверьте, верное ли оно.</p>");
                }
                else
                {
                    ModelState.AddModelError("", result);
                    return PartialView("_AddWork", workOnTask);
                }

                return RedirectToLocal($"~/Tasks/TaskDetails/{workOnTask.TaskId}");
            }
            return PartialView("_AddWork", workOnTask);
        }

        [Authorize]
        [OnlyClient]
        [HttpPost]
        public ActionResult CreateTask(CreateTask task)
        {
            if (ModelState.IsValid)
            {
                var checkCategory = unitOfWork.Category.GetItemById(task.CategoryId);
                var userInfo = unitOfWork.Users.GetItemById(int.Parse(User.Identity.Name));
                if (checkCategory == null)
                {
                    ModelState.AddModelError("", "Недопустимое значение для поля \"Категория\"");

                    return View();
                }
                if (userInfo.Money < task.Price)
                {
                    return RedirectToAction("NoMoneyNoHoney", "Account");
                }
                unitOfWork.Tasks.AddNewItem(TaskModelInit(task));
                unitOfWork.Save();

                return RedirectToAction("MyTasks", "Account");
            }

            return View();
        }

        [Authorize]
        public ActionResult EditTask(int? id)
        {
            if (id != null)
            {
                var task = unitOfWork.Tasks.GetItemById(id);

                if (task.UserId != int.Parse(User.Identity.Name) || task == null)
                {
                    return HttpNotFound();
                }

                var category = unitOfWork.Category.GetAll();
                ViewBag.CategoryId = new SelectList(category, "CategoryId", "CategoryName", task.CategoryId);

                return View(CreateTaskModelInit(task));
            }
            else
            {
                return HttpNotFound();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditTask(CreateTask createTask)
        {
            var task = unitOfWork.Tasks.GetItemById(createTask.TaskId);
            var category = unitOfWork.Category.GetAll();
            ViewBag.CategoryId = new SelectList(category, "CategoryId", "CategoryName", createTask.CategoryId);
            unitOfWork.Dispose();
            if (ModelState.IsValid)
            {
                if (task != null)
                {
                    unitOfWork = new UnitOfWork();
                    unitOfWork.Tasks.UpdateItem(TaskModelInit(createTask));
                    unitOfWork.Save();

                    return RedirectToAction("MyTasks", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка");
                }
            }

            return View(task);
        }

        [Authorize]
        public ActionResult DeleteTask(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var task = unitOfWork.Tasks.GetItemById(id);

            if (task.UserId != int.Parse(User.Identity.Name))
            {
                return HttpNotFound();
            }

            unitOfWork.Tasks.DeleteItem(id);
            unitOfWork.Save();

            return RedirectToAction("MyTasks", "Account");
        }

        [Authorize]
        public FileResult DownoloadFile(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var work = unitOfWork.WorkOnTask.GetItemById(id);
            string path = Server.MapPath("~/Content/Assets/Tasks/" + work.File);
            string[] filename = work.File.Split('/');
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename[1]);
        }

        [Authorize]
        private Tasks TaskModelInit(CreateTask task)
        {
            Tasks tasks = new Tasks 
            { 
                TaskName = task.TaskName, 
                CategoryId = task.CategoryId, 
                Description = task.Description,
                Price = task.Price,
                PublishDate = DateTime.Now, 
                UserId = int.Parse(User.Identity.Name),
                TaskId = task.TaskId,
                Status = "Размещен"
            };
            return tasks;
        }

        [Authorize]
        private CreateTask CreateTaskModelInit(Tasks task)
        {
            CreateTask tasks = new CreateTask
            {
                TaskName = task.TaskName,
                CategoryId = task.CategoryId,
                Description = task.Description,
                Price = task.Price,
                TaskId = task.TaskId
            };
            return tasks;
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