using SlothFreelance.Models;
using SlothFreelance.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlothFreelance.AccountController
{
    public class MoneyAccount
    {
        private UnitOfWork _unitOfWork;

        public MoneyAccount(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddMoney(int id, MoneyModel moneyModel)
        {
            var user = _unitOfWork.Users.GetItemById(id);
            _unitOfWork.Dispose();
            _unitOfWork = new UnitOfWork();
            user.Money += moneyModel.Money;
            _unitOfWork.Users.UpdateItem(user);
            _unitOfWork.Save();
        }

        public string TaskPayment(Users client, Users executer, int price)
        {
            if (client.Money < price)
            {
                return "Недостаточно средств на счету";
            }
            else
            {
                client.Money -= price;
                executer.Money += price;
                _unitOfWork.Users.UpdateItem(client);
                _unitOfWork.Users.UpdateItem(executer);
                _unitOfWork.Save();
                return null;
            }
        }
    }
}