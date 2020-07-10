using SlothFreelance.Models;
using SlothFreelance.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlothFreelance
{
    public abstract class MyBasePage : WebViewPage<Users>
    {
        public UnitOfWork unitOfWork;

        protected override void InitializePage()
        {
            unitOfWork = new UnitOfWork();
        }
    }

    /*
     @inherits SlothFreelance.MyBasePage
     @{
        var user = unitOfWork.Users.GetItemById(int.Parse(User.Identity.Name));
     }
     */
}