using G7_SistemaWeb.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Net.Http.Json;
using System.Net;
using System.Net.Http.Headers;

namespace G7_SistemaWeb.Models
{
    public class UserModel
    {
        public Answer registrarUsuario(User user) //Metodo para registrar usuario en api *NO IMPLEMENTADO*
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "Registro"; //genera el link del api
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); //Serializa el objeto, es decir lo vuelve un JSON

                using(HttpClient client = new HttpClient()) //llama al cliente, es un metodo anonimo por lo tanto no lleva token
                {
                    HttpResponseMessage resp = client.PostAsync(url, content).Result; //manda la peticion


                    if (resp.IsSuccessStatusCode) //Si es correcta
                    {
                        Answer answer = resp.Content.ReadFromJsonAsync<Answer>().Result; //lee la respuesta
                        return answer;
                    }
                    else //Sino igualmente la lee
                    {
                        Answer answer = resp.Content.ReadFromJsonAsync<Answer>().Result;
                        return answer;
                    }
                }
            }
            catch (Exception ex) //En caso de error retorna nulo
            {
                return null;
                throw ex;
            }
        }

        public Answer registrarUsuarioAdmin(User user) //Para panel de administrador *NO IMPLEMENTADO*
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "Registro/admin"; //genera el link
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); //Serializa el objeto

                using (HttpClient client = new HttpClient()) //llama al cliente
                {
                    var token = HttpContext.Current.Session["Token"] as string; //genera el token desde la variable de sesion
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); //la pasa como un header
                    HttpResponseMessage resp = client.PostAsync(url, content).Result; //llama a la peticion en el api


                    if (resp.IsSuccessStatusCode) //Si es correcto o no lee la respuesta
                    {
                        Answer answer = resp.Content.ReadFromJsonAsync<Answer>().Result;
                        return answer;
                    }
                    else
                    {
                        Answer answer = resp.Content.ReadFromJsonAsync<Answer>().Result;
                        return answer;
                    }
                }
            }
            catch (Exception ex) //En caso de error
            {
                return null;
                throw ex;
            }
        }

        public UserAnswer login(User user) //Metodo utilizado en el login
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "login"; //genera el link de la peticion
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); //serializa el objeto

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = client.PostAsync(url, content).Result; //no ocupa token, aqui manda la peticion


                    if (resp.IsSuccessStatusCode) //Si es correcto lee respuesta y sino tampoco, se encuentra validado en el api
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                    else
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                }
            }
            catch (Exception ex) //En caso de error
            {
                return null;
                throw ex;
            }
        }

        public UserAnswer resetPassword(User user) //Metodo para resetear la contrase;a, genera una temporal *NO IMPLEMENTADO*
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "resetPassword";
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = client.PostAsync(url, content).Result;


                    if (resp.IsSuccessStatusCode)
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                    else
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        public UserAnswer setPassword(User user) //Para cambiar la contrase;a despues del ingreso con la temporal, *NO IMPLEMENTADO*
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "setPassword"; //genera el link de conexion con el api
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); //Serializa el objeto, lo pasa a JSON

                using (HttpClient client = new HttpClient())
                {
                    var token = HttpContext.Current.Session["Token"] as string; //genera el token, ya que tuvo que iniciar sesion para validar la contrase;a temporal
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); // la coloca como un header
                    HttpResponseMessage resp = client.PostAsync(url, content).Result; //Envia la peticion


                    if (resp.IsSuccessStatusCode) //Lee la respuesta independientemente del resultado
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                    else
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                }
            }
            catch (Exception ex) //En caso de error
            {
                return null;
                throw ex;
            }
        }

        public UserAnswer updateUserAdmin(User user) //Para actualizar usuarios *NO IMPLEMENTADO*
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UrlApi"] + "updateUser/admin"; //genera el link
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");// serializa el objeto, lo pasa a JSON

                using (HttpClient client = new HttpClient())
                {
                    var token = HttpContext.Current.Session["Token"] as string; //lee el token de la sesion
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); //lo coloca en la peticion
                    HttpResponseMessage resp = client.PutAsync(url, content).Result; //realiza la peticion


                    if (resp.IsSuccessStatusCode) //lee respuesta independientemente del resultado
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
                    }
                    else
                    {
                        UserAnswer answer = resp.Content.ReadFromJsonAsync<UserAnswer>().Result;
                        return answer;
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