using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace G7_WebApi.Utilities
{
    internal class TokenHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) //Metodo que genera las diferentes respuestas con respecto al token
        {
            HttpStatusCode statusCode;
            string token;

            if(!TryRetrieveToken(request, out token)) //Envia a verificar la estructura del token
            {
                statusCode = HttpStatusCode.Unauthorized; //Si la estructura no es valida o el token no existe devuelve Sin Autorizacion
                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                var secretKey = ConfigurationManager.AppSettings["Secret-Key"]; //Extrae la contrase;a desde el web config
                var issuerToken = ConfigurationManager.AppSettings["Issuer"]; //Quien emite la respuesta
                var audienceToken = ConfigurationManager.AppSettings["Audience"]; //Quien recibe la respuesta

                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey)); //Encripta la contrase;a

                SecurityToken securityToken;

                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler(); //Usa el handler para verificar el token recibido
                TokenValidationParameters validationParameters = new TokenValidationParameters() //Verifica los parametros del token recibido
                {
                    ValidAudience = audienceToken, //Quien recibe respuesta
                    ValidIssuer = issuerToken, //Quien realiza la respuesta
                    ValidateLifetime = true, //Habilita la verificacion de tiempo de vida del token
                    ValidateIssuerSigningKey = true, //Habilita la verificacion por contrase;a desde el que emite la peticion
                    LifetimeValidator = this.LifetimeValidator, //Tiempo de expiracion valido
                    IssuerSigningKey = securityKey //verifica la contrase;a
                };

                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken); 
                HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken); //Si es correcto, genera una respuesta autorizada y permite continuar el flujo de recurso solicitado
            }catch (Exception ex) //En caso de que exista algun error, genera un internal server error
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { }); //Se maneja mediante hilos por lo tanto, se encuentra en cosntante funcionamiento enviando respuestas a cada solicitud, si llega a este punto existe un error.
        }

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token) //Valida la estructura del token
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1) //Valida que el header no venga vacio
            {
                return false;
            }

            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken; //Verifica que antes del token exista la palabra bearer
            return true;
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) //valida el tiempo de expiracion del token
        {
            var temp = false;

            if(expires.HasValue && DateTime.UtcNow < expires && (notBefore.HasValue && DateTime.UtcNow > notBefore)) //Condicional para verificar el tiempo de expiracion, si es correcto, retorna true
            {
                temp = true;
            }

            return temp;
        }

    }
}