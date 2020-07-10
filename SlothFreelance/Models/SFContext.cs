using System;
using System.Data.Entity;

namespace SlothFreelance.Models
{
    public class SFContext : DbContext
    {
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TaskRequests> TaskRequests { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<WorkOnTask> WorkOnTasks { get; set; }
    }

    public class SFDbInitializer : DropCreateDatabaseIfModelChanges<SFContext>
    {
        protected override void Seed(SFContext context)
        {
            context.Roles.Add(new Roles { RoleName = "Исполнитель" });
            context.Roles.Add(new Roles { RoleName = "Заказчик" });

            context.Users.Add(new Users { UserId = 1, UserName = "Ramin", Country = "Россия", City = "Ижевск", PhoneNumber = "+79828256346", Email = "ram.guseunov@gmail.com", Password = "ramin2585", RoleId = 2, Image = null, Money = 0 });
            context.Users.Add(new Users { UserId = 2, UserName = "Sergey", Country = "Украина", City = "Киев", PhoneNumber = "+79828254663", Email = "seriy@gmail.com", Password = "ramin2585", RoleId = 1, Image = null, Money = 0 });
            context.Users.Add(new Users { UserId = 3, UserName = "Егор", Country = "Украина", City = "Киев", PhoneNumber = "+79828254663", Email = "mr.lol500@mail.ru", Password = "ramin2585", RoleId = 1, Image = null, Money = 0 });

            context.Categories.Add(new Categories { CategoryName = "Дизайн" });
            context.Categories.Add(new Categories { CategoryName = "IT" });
            context.Categories.Add(new Categories { CategoryName = "Тексты и переводы" });
            context.Categories.Add(new Categories { CategoryName = "Музыка и видео" });

            context.Tasks.Add(new Tasks { TaskName = "Сделать сайт по готовому дизайну", Description = "Требуется сделать сайт по готовому дизайну, скину на него ссылку.", Price = 3000, CategoryId = 2, PublishDate = DateTime.Now, Status = "Размещен", UserId = 1 });
            context.Tasks.Add(new Tasks { TaskName = "Сделать логотип для сайта", Description = "Требуется сделать логотип для моего будущего сайта. Справитесь?", Price = 2000, CategoryId = 1, PublishDate = DateTime.Now, Status = "Размещен", UserId = 1 });

            base.Seed(context);
        }
    }
}