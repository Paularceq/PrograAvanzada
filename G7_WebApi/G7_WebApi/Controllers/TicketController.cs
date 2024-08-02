using G7_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace G7_WebApi.Controllers
{
    [Authorize]
    public class TicketController : ApiController
    {
        private G7_DATABASEEntities db = new G7_DATABASEEntities();

        [HttpGet]
        [Route("api/allTickets")]
        public HttpResponseMessage allTickets() //Metodo para obtener todos los tickets (uso de administrador)
        {
            try
            {
                var tickets = db.TICKETs.Where(x => x.ACTIVE == 1).ToList(); //Se trae todos los tickets que se encuentren activos

                if(tickets.Count > 0) //Si conteo es mayor a 0
                {
                    return Request.CreateResponse(HttpStatusCode.OK, tickets); //devuelve respuesta positiva
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron tickets activos..."); //Si el conteo = 0 significa que no encontro nada
                }
            }catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message); //Si se cae error del servidor
            }
        }

        [HttpGet]
        [Route("api/allMyTickets/{usu_id}")]
        public HttpResponseMessage allMyTickets(string usu_id) //Metodo para obtener todos los tickets (uso de comun)
        {
            try
            {
                var tickets = db.TICKETs.Where(x => x.ACTIVE == 1 && x.USU_OPEN == usu_id).ToList(); //Se trae todos los tickets que se encuentren activos y que sean abiertos por el usuario

                if (tickets.Count > 0) //Si conteo es mayor a 0
                {
                    return Request.CreateResponse(HttpStatusCode.OK, tickets); //devuelve respuesta positiva
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron tickets activos..."); //Si el conteo = 0 significa que no encontro nada
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message); //Si se cae error del servidor
            }
        }

        [HttpGet]
        [Route("api/MyActiveTickets/{usu_id}")]
        public HttpResponseMessage myActiveTickets(string usu_id) //Extrae los tickets 
        {
            try
            {
                var tickets = db.TICKETs.Where(x => x.USU_OPEN == usu_id && x.ID_STATE < 5).ToList(); //Extrae los tickets que se encuentren activos (en proceso)

                if(tickets.Count > 0) //Si encuentra
                {
                    return Request.CreateResponse(HttpStatusCode.OK, tickets); //los retorna
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron tickets activos..."); //Sino envia mensaje de alerta
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message); //En caso de error, error del servidor
            }
        }

        [HttpGet]
        [Route("api/archiveTickets/{usu_id}")] //tickets archivados por el usuario
        public HttpResponseMessage MyArchivedTickets(string usu_id)
        {
            try
            {
                var tickets = db.TICKETs.Where(x => x.ACTIVE == 1 && x.ID_STATE == Convert.ToInt32(ConfigurationManager.AppSettings["archived-state"]) && x.USU_OPEN == usu_id).ToList();
                //Realiza la busqueda por id de estado archivado y que sean creados por el usuario
                if (tickets.Count > 0) //Si encuentra algo
                {
                    return Request.CreateResponse(HttpStatusCode.OK, tickets); //lo devuelve
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron tickets archivados..."); //sino devuelve alerta
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message); //En caso de que se caiga, error del servidor
            }
        }

        [HttpGet]
        [Route("api/archiveTickets/admin")] //Metodo de archivados (admin)
        public HttpResponseMessage MyArchivedTicketsAdmin()
        {
            try
            {
                //Busca todos los tickets con id de archivado y activos
                var tickets = db.TICKETs.Where(x => x.ACTIVE == 1 && x.ID_STATE == Convert.ToInt32(ConfigurationManager.AppSettings["archived-state"])).ToList();

                if (tickets.Count > 0) //Si encuentra, los devuelve
                {
                    return Request.CreateResponse(HttpStatusCode.OK, tickets);
                }
                else
                { //Sino devuelve un not found de respuesta
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No se encontraron tickets archivados...");
                }
            }
            catch (Exception ex)
            { //En caso de que se caiga, error del servidor
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/CreateTicket")] 
        public HttpResponseMessage createRole(TICKET entity) //Metodo para crear los ticket
        {
            try
            {
                entity.ACTIVE = 1; //Asigna el ticket nuevo como activo

                if (ModelState.IsValid) //Valida que el modelo obtenido sea valido
                {
                    db.TICKETs.Add(entity); //Lo añade a la base de datos
                    db.SaveChanges(); //Lo guarda en la base de datos
                    return Request.CreateResponse(HttpStatusCode.OK, "Se ha creado el ticket satisfactoriamente..."); //Devuelve respuesta exitosa
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
        [Route("api/UpdateRole/{id}/{usu_id}")] //Actualizar ticket
        public HttpResponseMessage UpdateTicket(long id, TICKET entity, string usu_id)  //Id del ticket y la entidad modificada
        {
            try
            {
                if (entity != null)
                {
                    db.Entry(entity).State = EntityState.Modified; //Actualiza la entidad
                    db.SaveChanges(); //Guarda Cambios

                    return Request.CreateResponse(HttpStatusCode.OK, "Se ha actualizado correctamente el ticket!"); //Si actualiza correctamente
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
        [Route("api/DeleteTicket/{id}/{usu_id}")] //Elimina el ticket de forma logica
        public HttpResponseMessage DeleteTicket(long id, string usu_id)
        {
            try
            {
                var ticket = db.TICKETs.Find(id); //Busca el rol a eliminar

                if (ticket != null)
                {
                    ticket.USU_DELETE = usu_id;
                    ticket.ACTIVE = 0; //cambia el estado a inactivo
                    db.Entry(ticket).State = EntityState.Modified; //Actualiza
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

