using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace G7_SistemaWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var id = HttpContext.Session["cedula"] as string; //Extrae el id de la sesion
            if(id == null) //Si esta nulo redirecciona al login
            {
                return RedirectToAction("login", "User");
            }
            return View();
        }
    }
}