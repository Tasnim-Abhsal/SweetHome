using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Controllers
{
    public class LoginSignupController : Controller
    {
        // GET: LoginSignup
        HotelDBEntities entity = new HotelDBEntities();
        public ActionResult AdminLogin()
        {
            return View();
        }
        public ActionResult AdminSignup()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult AdminLogin(AdminLoginViewModel credentials)
        {
            bool userExist = entity.AdminLogins.Any(x => x.Username == credentials.Username && x.Password == credentials.Password);
            AdminLogin a = entity.AdminLogins.FirstOrDefault(x => x.Username == credentials.Username && x.Password == credentials.Password);
            if (userExist)
            {
                FormsAuthentication.SetAuthCookie(a.Username, false);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Username or Password is wrong");
            return View();
        }
        [HttpPost]
        public ActionResult AdminSignup(AdminLogin userinfo)
        {
            entity.AdminLogins.Add(userinfo);
            entity.SaveChanges();
            return RedirectToAction("AdminLogin");
        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("AdminLogin");
            
        }
        

    }
}