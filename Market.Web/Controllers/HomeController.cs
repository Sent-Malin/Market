using Market.Data;
using Market.Data.Models;
using Market_Web.Pages;
using Market_Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Market_Web.Controllers
{
    public class HomeController : Controller
    {
        MarketDbContext db=new MarketDbContext();
        public ActionResult Index(Data data)
        {
            ViewData["Title"] = "Home Page";
            if (data.typeError==Errors.ErrorInAuth)
            {
                LoginModel lm = new LoginModel(data.errorMessage, Errors.ErrorInAuth);
                return View(lm);
            } else if(data.typeError ==Errors.ErrorInReg)
            {
                return View(new LoginModel(data.errorMessage, Errors.ErrorInReg));
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string authName, string authPass)
        {
            UsersEntity? us = db.Users.Select(r => r)
                .Where(n => n.Name == authName)
                .FirstOrDefault();
            
            if ((us != null)&&(us.Password == authPass))
            {
                if (us.IsOnline == false)
                {
                    us.IsOnline = true;
                    db.SaveChanges();
                    return RedirectToAction("Game", us);
                }
                return RedirectToAction("Index", "Home", new Data("Запрашиваемый пользователь уже в сети", Errors.ErrorInAuth));
            }
            return RedirectToAction("Index", "Home", new Data("Неверное имя пользователя или пароль", Errors.ErrorInAuth));
        }

        public ActionResult Registration(string regName, string regPass, string regPassRepeat)
        {
            if (regPass != regPassRepeat)
            {
                return RedirectToAction("Index", new Data("Пароль и повтор пароля не совпадают", Errors.ErrorInReg));
            }

            UsersEntity? us = db.Users.Select(r => r)
                .Where(n => n.Name == regName)
                .FirstOrDefault();

            if (us == null)
            {
                UsersEntity user = new UsersEntity(regName, regPass, 0, 0, 100);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Game", user);
            }
            return RedirectToAction("Index", new Data("Имя пользователя занято", Errors.ErrorInReg));
        }

        [HttpGet]
        public ActionResult Game(UsersEntity data)
        {
            ViewData["Title"] = "Home Page";
            return View(data);
        }

        [HttpPost]
        public ActionResult ComeBackGame(UsersEntity user) 
        {
            UsersEntity? us = db.Users.Select(r => r)
                .Where(n => n.Name == user.Name)
                .FirstOrDefault();
            return RedirectToAction("Game", us);
        }
    }
}
