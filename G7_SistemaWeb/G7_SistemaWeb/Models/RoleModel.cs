using G7_SistemaWeb.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace G7_SistemaWeb.Models
{
    public class RoleModel
    {
        public List<ROLE> extraerRoles() //Extrae todos los roles desde el api *NO IMPLEMENTADO*
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "AllRoles"; //Genera el link de conexion con el api
                using (HttpClient client = new HttpClient()) //Llama al cliente
                {
                    var token = HttpContext.Current.Session["Token"] as string; //Extrae el token generada en la variable de sesion
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); //Lo manda como un header
                    HttpResponseMessage resp = client.GetAsync(url).Result; //Realiza la peticion

                    if (resp.IsSuccessStatusCode)// Si es correcto, 
                    {
                        List<ROLE> roles = resp.Content.ReadFromJsonAsync<List<ROLE>>().Result; //Convierte la respuesta en una lista

                        if (roles.Count < 1) //Valida que no venga vacia
                        {
                            return null;
                        }
                        else //Si viene con objetos la retorna
                        {
                            return roles;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex) //en caso de error
            {
                return null;
                throw ex;
            }
        }
    }
}