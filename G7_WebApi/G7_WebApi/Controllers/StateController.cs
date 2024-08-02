using G7_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace G7_WebApi.Controllers
{
    [Authorize]
    public class StateController : ApiController
    {
        private G7_DATABASEEntities db = new G7_DATABASEEntities();

        [HttpGet]
        [Route("api/AllStates")] //Obtener los estados
        public HttpResponseMessage getStates()
        {
            try
            {
                var estados = db.STATE_TICKET.ToList().Where(x => x.ACTIVE == 1); //Extrae los estados activos

                if (estados != null && estados.Count() > 0) //Valida que encuentre algun estado
                {
                    return Request.CreateResponse(HttpStatusCode.OK, estados); //Si encuentra devuelve los estados con status correcto
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
        [Route("api/searchState/{id}")] //Extrae el estado solicitado
        public HttpResponseMessage getState(long id)
        {
            try
            {
                var estado = db.STATE_TICKET.FirstOrDefault(x => x.ID_STATE == id); //Busca el estado solicitado

                if (estado != null) //Valida que no venga vacio
                {
                    return Request.CreateResponse(HttpStatusCode.OK, estado); //Si lo encuentra devuelve ok y el estado solicitado
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
        [Route("api/CreateState")]
        public HttpResponseMessage createState(STATE_TICKET entity) //Metodo para crear los estados
        {
            try
            {
                entity.ACTIVE = 1; //Asigna el estado nuevo como activo

                if (ModelState.IsValid) //Valida que el modelo obtenido sea valido
                {
                    db.STATE_TICKET.Add(entity); //Lo añade a la base de datos
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
        [Route("api/UpdateState/{id}")] //Actualizar States
        public HttpResponseMessage UpdateState(long id, STATE_TICKET entity)  //Id del estado y la entidad modificada
        {
            try
            {
                if (entity != null)
                {
                    db.Entry(entity).State = EntityState.Modified; //Actualiza la entidad
                    db.SaveChanges(); //Guarda Cambios

                    return Request.CreateResponse(HttpStatusCode.OK, "Se ha actualizado correctamente el estado!"); //Si actualiza correctamente
                }

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Ha ocurrido un error al realizar la actualizacion!"); //Si trae algun error
            }
            catch (Exception e) //En caso de caida del sistema
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpDelete]
        [Route("api/DeleteState/{id}")] //Elimina el estado de forma logica
        public HttpResponseMessage DeleteState(long id)
        {
            try
            {
                var estado = db.STATE_TICKET.Find(id); //Busca el estado a eliminar

                if (estado != null)
                {
                    estado.ACTIVE = 0; //cambia el estado a inactivo
                    db.Entry(estado).State = EntityState.Modified; //Actualiza
                    db.SaveChanges(); //Guarda cambios
                    return Request.CreateResponse(HttpStatusCode.OK, "Se elimino correctamente el rol..");
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, "El recurso solicitado no existe.."); //En caso de que no lo encuentre
            }
            catch (Exception e) //En caso de error
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
