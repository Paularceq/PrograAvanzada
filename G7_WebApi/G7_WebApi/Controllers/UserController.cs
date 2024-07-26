using G7_WebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using G7_WebApi.Utilities;


namespace G7_WebApi.Controllers
{
    public class UserController : ApiController
    {
        private G7_DATABASEEntities db = new G7_DATABASEEntities();

        [AllowAnonymous] //Debe ser anonimo ya que es la creacion de usuarios 
        [HttpPost]
        [Route("api/Registro")] 
        public HttpResponseMessage crearUsuario(User user)
        {
            try
            {
                var answer = new Answer(); //Inicializa objeto para brindar la respuesta
                user.ACTIVE = 1; //Coloca su estado en activo
                user.ID_ROLE = Convert.ToInt32(ConfigurationManager.AppSettings["user_role"]); //Coloca el perfil de usuario comun, solo tiene permiso para creacion y lectura de sus tickets
                user.PASSWD = Helper.Encrypt(user.PASSWD);//Cambia la contraseña ingresada por una version encriptada

                if (ModelState.IsValid) //Valida que el modelo sea valido antes de enviar a la base de datos
                {
                    db.Users.Add(user); //Agrega el usuario a la base de datos
                    db.SaveChanges(); //Guarda en base de datos
                    answer.Code = "1";
                    answer.Mensaje = "Usuario Creado Correctamente";
                    return Request.CreateResponse(HttpStatusCode.OK, answer); //Si es correcto crea el objeto de respuesta con codigo correcto
                }
                else
                {
                    answer.Code = "-1";
                    answer.Mensaje = "El modelo del usuario no es valido...";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, answer); //Si el modelo no es valido, crea un error bad request y retorna de respuesta codigo invalido
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); //En caso de el sistema se caiga, envia error de servidor
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/Registro/admin")] //Este metodo de registro funciona solo para el panel de administrador
        public HttpResponseMessage crearUsuarioAdmin(User user)
        {
            try
            {
                var answer = new Answer();
                user.ACTIVE = 1; //Mantiene el estado activo para el nuevo usuario
                user.PASSWD = Helper.GenerateTempPassword(); //Ya que es un usuario creado por el administrador se genera una contraseña aleatoria
                user.TEMP_PASSWD = user.PASSWD; //La contraseña generada es temporal por que se setea tambien

                if (ModelState.IsValid) //Si el modelo es valido
                {
                    db.Users.Add(user); //Agrego a la base de datos
                    db.SaveChanges();//Guarda los cambios en base de datos
                    answer.Code = "1";
                    answer.Mensaje = "Usuario Creado Correctamente";
                    Helper.SendEmail(ConfigurationManager.AppSettings["mailSMTP"], 
                        ConfigurationManager.AppSettings["asuntoSMTP"], 
                        "Su contraseña temporal para el sistema es: " + user.PASSWD); //Cuando fue agregado, envia un correo con las credenciales temporales del nuevo usuario

                    return Request.CreateResponse(HttpStatusCode.OK, answer); //Retorna respuesta positiva
                }
                else
                { //Si no es valido
                    answer.Code = "-1";
                    answer.Mensaje = "El modelo del usuario no es valido...";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, answer); //Retorna el error 
                }
            }
            catch (Exception ex) //Si se cae el sistema
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); //Se coloca como un error del servidor
            }
        }

        [HttpPost]
        [AllowAnonymous] //Va anonimo ya que la persona persona en este momento no tiene credenciales en el sistema
        [Route("api/login")]
        public HttpResponseMessage Login(User entity)
        {
            try
            {
                entity.PASSWD = Helper.Encrypt(entity.PASSWD); //La contraseña enviada desde el formulario se debe encriptar para validarla en base de datos

                var usuario = db.Users.Where(x => x.EMAIL == entity.EMAIL && x.PASSWD == entity.PASSWD).FirstOrDefault(); //Se realiza la busqueda para verificar si el usuario existe

                if (usuario != null) //Valida que lo haya encontrado
                {
                    usuario.ROLE.Users = null; //Limpia un poco las referencias circulares que crea entity framework

                    UserAnswer answer = new UserAnswer() //Crea el objeto de respuesta a la peticion 
                    {
                        user = usuario,
                        Codigo = "1",
                        Mensaje = "Credenciales Validas",
                        Token = GenerateJWT(usuario) //Mando a generar el token para el usuario, necesario para el resto de funciones
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, answer); //retorno la respuesta
                }

                else //No encontro el usuario
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new UserAnswer //Dado que no sabemos si se equivoco o del todo no existe 
                    {
                        Codigo = "-1",
                        Mensaje = "Crendenciales Incorrectas!" //Retornamos un mensaje de error
                    });
                }
            }
            catch (Exception ex) //En caso de que se caiga el sistema
            {
                return Request.CreateErrorResponse (HttpStatusCode.InternalServerError, ex); //Retorno  error del servidor
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("api/Informacion")]
        public HttpResponseMessage Informacion()
        {
            var user = new User()
            {
                USU_ID = "123456789",
                    FIRST_NAME = "Juan",
                    LAST_NAME = "Montoya",
                    EMAIL = "jmontoya@ufide.ac.cr",
                    PASSWD = Helper.GenerateTempPassword()
                };

                var token = GenerateJWT(user);

            Helper.SendEmail(ConfigurationManager.AppSettings["mailSMTP"], ConfigurationManager.AppSettings["asuntoSMTP"], "Su contraseña para el sistema es: " + user.PASSWD);

            return Request.CreateResponse(HttpStatusCode.OK, new UserAnswer
                {
                    user = user,
                    Codigo = "200",
                    Mensaje = "Token Generado",
                    Token = token
                });

            }

        private string GenerateJWT(User user) //Generar el token
        {
            var secretKey = ConfigurationManager.AppSettings["Secret-Key"]; //Extrae contrase;a del web config
            var _issuer = ConfigurationManager.AppSettings["Issuer"]; //Quien recibe peticion
            var _audience = ConfigurationManager.AppSettings["Audience"]; //Quien recibe respuesta

            if (!Int32.TryParse(ConfigurationManager.AppSettings["Expires"], out int expire)) //Genera el tiempo de expiracion
            {
                expire = 2;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); //Encripta la contrase;a
            var credenciales = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); //Lo genera como una credencial de ingreso de 256 Bytes
            var header = new JwtHeader(credenciales); //Agrega esas credenciales al header

            var _claims = new[] //Genera los diferentes claims necesarios
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Este es el unico obligatorio, forma parte de la estructura del token
                new Claim(JwtRegisteredClaimNames.NameId, user.USU_ID), //Claim que almacena la cedula del usuario
                new Claim(JwtRegisteredClaimNames.Email, user.EMAIL), //Claim que almacena el email de usuario
                new Claim("Nombre Completo", user.FIRST_NAME + " " + user.LAST_NAME) //Claim que almacena el nombre completo del usuario
            };

            var payload = new JwtPayload( //Crea el payload con las restricciones del token
                issuer: _issuer,
                audience: _audience,
                claims: _claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(expire)
                );

            var _token = new JwtSecurityToken(header, payload); //almacena todo el un token sin escribir

            return new JwtSecurityTokenHandler().WriteToken(_token); //convierte la informacion guardada en el token
        }
    }
}
