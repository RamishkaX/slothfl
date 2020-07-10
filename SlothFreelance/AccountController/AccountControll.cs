using SlothFreelance.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data.Entity;
using SlothFreelance.Models;

namespace SlothFreelance.AccountController
{
    public class AccountControll
    {
        private UnitOfWork _unitOfWork;

        public AccountControll(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void UserAuth(int id)
        {
            FormsAuthentication.SetAuthCookie(id.ToString(), true);
        }

        public bool UserRegister(RegisterModel registerModel)
        {
            _unitOfWork.Users.AddNewItem(new Users
            {
                UserName = registerModel.UserName,
                PhoneNumber = registerModel.PhoneNumber,
                Email = registerModel.Email,
                Password = registerModel.Password,
                RoleId = registerModel.RoleId,
                Image = null,
                Country = registerModel.Country,
                City = registerModel.City,
                Money = 0
            });

            _unitOfWork.Save();

            var user = _unitOfWork.Users.GetUserByLoginData(registerModel.Email, registerModel.Password);
            
            if (user != null)
            {
                UserAuth(user.UserId);
                return true;
            }

            return false;
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}