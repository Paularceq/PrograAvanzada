using G7_SistemaWeb.Entities;
using G7_SistemaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace G7_SistemaWeb.Controllers
{
    [OutputCache(NoStore = true, VaryByParam = "*", Duration = 0)]  //En teoria al cerrar sesion con esto no deberia permitir volver atras, sin embargo, lo permite, *preguntar al profe*
    public class UserController : Controller
    {
        private UserModel _user = new UserModel(); //Crea el objeto de modelo
        [HttpGet]
        public ActionResult login() //llama a la vista
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(User user)
        {
            HttpContext.Session.Clear(); //limpia variables de sesion
            if (user.EMAIL != null && user.PASSWD != null) //valida que venga la informacion
            {
                var resp = _user.login(user); //llama al metodo del modelo que conecta con el api

                if(resp.Codigo == "1") //Si la respuesta es correcta
                {
                    if(resp.user.TEMP_PASSWD == null) //valida si es una contrase;a temporal *NO IMPLEMENTADO AUN*
                    { //Si es contrase;a regular, ingresa las variables de sesion
                        HttpContext.Session["cedula"] = resp.user.USU_ID; 
                        HttpContext.Session["nombre"] = resp.user.FIRST_NAME + " " + resp.user.LAST_NAME;
                        HttpContext.Session["email"] = resp.user.EMAIL;
                        HttpContext.Session["rol"] = resp.user.ID_ROLE;
                        HttpContext.Session["Token"] = resp.Token;

                        return RedirectToAction("Index", "Home"); //Redirecciona al home
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");  //Como aun no se implementa cambiar passwd, redirecciona al home
                    }
                }
                else
                {
                    ViewBag.error = "Usuario o/y contraseña incorrectos.."; //En caso de que la respuesta sea != de 1 imprime en la vista error
                    return View();
                }

            }
            else
            {
                ViewBag.error = "No se ha proporcionado los datos necesarios..."; //En caso de que el modelo no sea valido, imprime en la vista error
                return View();
            }
        }

        public ActionResult logout() //metodo para cerrar sesion, falta verificar xq al volver atras a pesar del cache sigue permitiendo ver la informacion
        {
            Session.Abandon();
            HttpContext.Session.Clear();
            return RedirectToAction("login", "User");
        }
    }
}