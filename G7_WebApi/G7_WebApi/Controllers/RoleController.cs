using G7_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace G7_WebApi.Controllers
{
    public class RoleController : ApiController
    {
        private G7_DATABASEEntities db = new G7_DATABASEEntities();

        [HttpGet]
        [Route("api/AllRoles")] //Obtener los roles
        public HttpResponseMessage getRoles()
        {
            try
            {
                var roles = db.ROLES.ToList().Where(x => x.ACTIVE == 1); //Extrae los roles activos

                if (roles != null && roles.Count() > 0) //Valida que encuentre algun rol
                {
                    return Request.CreateResponse(HttpStatusCode.OK, roles); //Si encuentra devuelve los roles con status correcto
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No se pudo obtener la informacion solicitada..."); //Sino, error 404 y mensaje de error
                }
            }
            catch (Exception ex) //Cualquier otro error
            {
                Console.WriteLine(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ha ocurrido un error con el servidor.."); //Error con el servidor
            }
        }

        [HttpGet]
        [Route("api/searchRole/{id}")] //Extrae el rol solicitado
        public HttpResponseMessage getRole(long id)
        {
            try
            {
                var role = db.ROLES.FirstOrDefault(x => x.ID_ROLE == id); //Busca el rol solicitado

                if (role != null) //Valida que no venga vacio
                {
                    return Request.CreateResponse(HttpStatusCode.OK, role); //Si lo encuentra devuelve ok y el rol solicitado
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No se encontro el rol solicitado..."); //Sino, error 404 y mensaje de error
                }
            }
            catch (Exception ex) //Cualquier otro error
            {
                Console.WriteLine(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ha ocurrido un error con el servidor..."); //Error en el servidor
            }
        }

        [HttpPost]
        [Route("api/CreateRole")]
        public HttpResponseMessage createRole(ROLE entity) //Metodo para crear los roles
        {
            try
            {
                entity.ACTIVE = 1; //Asigna el rol nuevo como activo

                if (ModelState.IsValid) //Valida que el modelo obtenido sea valido
                {
                    db.ROLES.Add(entity); //Lo añade a la base de datos
                    db.SaveChanges(); //Lo guarda en la base de datos
                    return Request.CreateResponse(HttpStatusCode.OK, "Se ha creado el rol satisfactoriamente..."); //Devuelve respuesta exitosa
                }
                else //Si no es valido devuelve error
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No se proporciono la informacion necesaria.."); //Mensaje de error
                }
            }
            catch (Exception ex) //Cualquier otro error
            {
                Console.WriteLine(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Ha ocurrido un error en el servidor..."); //Error en el servidor
            }
        }

        [HttpPut]
        [Route("api/UpdateRole/{id}")] //Actualizar Roles
        public HttpResponseMessage UpdateRole(long id, ROLE entity)  //Id del rol y la entidad modificada
        {
            try 
            {
                if(entity != null)
                {
                    db.Entry(entity).State = EntityState.Modified; //Actualiza la entidad
                    db.SaveChanges(); //Guarda Cambios

                    return Request.CreateResponse(HttpStatusCode.OK, "Se ha actualizado correctamente el rol!"); //Si actualiza correctamente
                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Ha ocurrido un error al realizar la actualizacion!"); //Si trae algun error
            }catch(Exception e) //En caso de caida del sistema
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpDelete]
        [Route("api/DeleteRole/{id}")] //Elimina el Rol de forma logica
        public HttpResponseMessage DeleteRole(long id)
        {
            try
            {
                var rol = db.ROLES.Find(id); //Busca el rol a eliminar

                if(rol != null)
                {
                    rol.ACTIVE = 0; //cambia el estado a inactivo
                    db.Entry(rol).State = EntityState.Modified; //Actualiza
                    db.SaveChanges(); //Guarda cambios
                    return Request.CreateResponse(HttpStatusCode.OK, "Se elimino correctamente el rol..");
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "El recurso solicitado no existe.."); //En caso de que no lo encuentre
            }catch (Exception e) //En caso de error
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

    }
}
