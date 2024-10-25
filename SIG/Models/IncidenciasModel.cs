using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace SIG.Models
{
    public class IncidenciasModel
    {
        public bool InsertarTicket(Entidades.Ticket ticket)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                //rowsAffected = context.InsertarTicket(idSesion, ticket.titulo, ticket.descripcion, ticket.tipo_incidencia, ticket.comentarios_usuario, DateTime.Now, 1);
                var tTicket = new BaseDatos.Ticket();
                tTicket.id_usuario = idSesion;
                tTicket.titulo = ticket.titulo;
                tTicket.descripcion = ticket.descripcion;
                tTicket.tipo_incidencia = ticket.tipo_incidencia;
                tTicket.comentarios_usuario = ticket.comentarios_usuario;
                tTicket.fecha_registro_usuario = DateTime.Now;
                tTicket.estado = 1;

                context.Ticket.Add(tTicket);
                rowsAffected = context.SaveChanges();
            }
            return (rowsAffected > 0 ? true : false);
        }

        public List<TablaTicket> ListaIncidenciasUsuario()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return (from x in context.Ticket
                        join t in context.Ticket_Tipo on x.tipo_incidencia equals t.tipo_incidencia
                        join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                        where x.id_usuario == idSesion && x.estado < 4
                        select new TablaTicket
                        {
                            id_ticket = x.id_ticket,
                            fecha_registro_usuario = (DateTime)x.fecha_registro_usuario,
                            tipo_incidencia = t.descripcion,
                            estado = e.descripcion
                        }).ToList(); 
            }
        }

        public BaseDatos.Ticket verTicket(int idTicket)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                int RolSesion = int.Parse(HttpContext.Current.Session["RolUsuario"].ToString());
                var query = (from x in context.Ticket
                             where x.id_ticket == idTicket && (x.id_usuario == idSesion || x.id_tecnico == idSesion || RolSesion == 3) //permisos para ver el ticket
                             select x).FirstOrDefault();

                if (query != null)
                    return query;
                else return null;
            }
        }

        public bool ActualizarTicket(BaseDatos.Ticket t)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {

                var ticket = context.Ticket.FirstOrDefault(x => x.id_ticket == t.id_ticket);

                if (ticket != null)
                {
                    if (ticket.estado != t.estado & t.estado != 0) ticket.estado = t.estado;

                    if (ticket.tipo_incidencia != t.tipo_incidencia & t.tipo_incidencia != 0) ticket.tipo_incidencia = t.tipo_incidencia;

                    if (ticket.prioridad != t.prioridad & t.prioridad != 0) ticket.prioridad = t.prioridad;

                    if (ticket.titulo != t.titulo & t.titulo != null) ticket.titulo = t.titulo;

                    if (ticket.descripcion != t.descripcion & t.descripcion != null) ticket.descripcion = t.descripcion;

                    if (ticket.comentarios_usuario != t.comentarios_usuario & t.comentarios_usuario != null) ticket.comentarios_usuario = t.comentarios_usuario;

                    if (ticket.fecha_registra_tecnico != t.fecha_registra_tecnico & t.fecha_registra_tecnico != null) ticket.fecha_registra_tecnico = t.fecha_registra_tecnico;

                    if (ticket.id_tecnico != t.id_tecnico & t.id_tecnico != 0) ticket.id_tecnico = t.id_tecnico;

                    if (ticket.comentario_tecnico != t.comentario_tecnico & t.comentario_tecnico != null) ticket.comentario_tecnico = t.comentario_tecnico;

                    if (ticket.fecha_cierre_ticket != t.fecha_cierre_ticket & t.fecha_cierre_ticket != null) ticket.fecha_cierre_ticket = t.fecha_cierre_ticket;
                }
                rowsAffected = context.SaveChanges();
            }

                return (rowsAffected > 0 ? true : false);
        }

        public bool CerrarTicket(Entidades.Ticket t)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var ticket = context.Ticket.FirstOrDefault(x => x.id_ticket == t.id_ticket);
                ticket.estado = 4;
                ticket.fecha_cierre_ticket = DateTime.Now;
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }

        public List<TablaTicket> ListaTicketsRegistrados()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Ticket
                        join t in context.Ticket_Tipo on x.tipo_incidencia equals t.tipo_incidencia
                        join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                        where x.estado == 1
                        select new TablaTicket
                        {
                            id_ticket = x.id_ticket,
                            fecha_registro_usuario = (DateTime)x.fecha_registro_usuario,
                            tipo_incidencia = t.descripcion,
                            estado = e.descripcion
                        }).ToList();
            }
        }

        public bool AsignarTicket(BaseDatos.Ticket t)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var ticket = context.Ticket.FirstOrDefault(x => x.id_ticket == t.id_ticket);

                if (ticket != null)
                {
                    if (ticket.tipo_incidencia != t.tipo_incidencia & t.tipo_incidencia != 0) ticket.tipo_incidencia = t.tipo_incidencia;

                    if (ticket.prioridad != t.prioridad & t.prioridad != 0) ticket.prioridad = t.prioridad;

                    if (ticket.id_tecnico != t.id_tecnico & t.id_tecnico != 0) ticket.id_tecnico = t.id_tecnico;

                    ticket.estado = 2;

                    ticket.fecha_registra_tecnico = DateTime.Now;

                }
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }

        public List<TablaTicket> ListaTicketsAsignados()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return (from x in context.Ticket
                        join t in context.Ticket_Tipo on x.tipo_incidencia equals t.tipo_incidencia
                        join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                        join p in context.Ticket_Prioridad on x.prioridad equals p.prioridad
                        where x.id_tecnico == idSesion && x.estado < 4
                        orderby x.prioridad descending
                        select new TablaTicket
                        {
                            id_ticket = x.id_ticket,
                            fecha_registro_usuario = (DateTime)x.fecha_registro_usuario,
                            tipo_incidencia = t.descripcion,
                            estado = e.descripcion,
                            prioridad = p.descripcion
                        }).ToList();
            }
        }

        public bool AtenderTicket(BaseDatos.Ticket t)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                // if estado es 4 actualizar fecha cierre a datetimenow
                var ticket = context.Ticket.FirstOrDefault(x => x.id_ticket == t.id_ticket);

                if (ticket != null)
                {
                    if (ticket.estado != t.estado & t.estado != 0) ticket.estado = t.estado;

                    if (ticket.tipo_incidencia != t.tipo_incidencia & t.tipo_incidencia != 0) ticket.tipo_incidencia = t.tipo_incidencia;

                    if (ticket.prioridad != t.prioridad & t.prioridad != null) ticket.prioridad = t.prioridad;

                    if (ticket.titulo != t.titulo & t.titulo != null) ticket.titulo = t.titulo;

                    if (ticket.descripcion != t.descripcion & t.descripcion != null) ticket.descripcion = t.descripcion;

                    if (ticket.comentarios_usuario != t.comentarios_usuario & t.comentarios_usuario != null) ticket.comentarios_usuario = t.comentarios_usuario;

                    if (ticket.fecha_registra_tecnico != t.fecha_registra_tecnico & t.fecha_registra_tecnico != null) ticket.fecha_registra_tecnico = t.fecha_registra_tecnico;

                    if (ticket.id_tecnico != t.id_tecnico & t.id_tecnico != null) ticket.id_tecnico = t.id_tecnico;

                    if (ticket.comentario_tecnico != t.comentario_tecnico & t.comentario_tecnico != null) ticket.comentario_tecnico = t.comentario_tecnico;

                    if (ticket.fecha_cierre_ticket != t.fecha_cierre_ticket & t.fecha_cierre_ticket != null) ticket.fecha_cierre_ticket = t.fecha_cierre_ticket;

                    if (ticket.fecha_cierre_ticket == null & t.estado == 4) ticket.fecha_cierre_ticket = DateTime.Now;
                }
                rowsAffected = context.SaveChanges();

            }
            return (rowsAffected > 0 ? true : false);
        }

        public List<TablaTicket> ListaHistoricoUsuario()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return (from x in context.Ticket
                        join t in context.Ticket_Tipo on x.tipo_incidencia equals t.tipo_incidencia
                        join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                        join p in context.Ticket_Prioridad on x.prioridad equals p.prioridad into prioridades
                        from p in prioridades.DefaultIfEmpty()
                        where x.id_usuario == idSesion && x.estado == 4
                        select new TablaTicket
                        {
                            id_ticket = x.id_ticket,
                            fecha_registro_usuario = (DateTime)x.fecha_cierre_ticket,
                            tipo_incidencia = t.descripcion,
                            estado = e.descripcion,
                            prioridad = p != null ? p.descripcion : "Sin prioridad"
                        }).ToList();
            }
        }

        public List<TablaTicket> ListaHistoricoAtendidos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return (from x in context.Ticket
                        join t in context.Ticket_Tipo on x.tipo_incidencia equals t.tipo_incidencia
                        join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                        join p in context.Ticket_Prioridad on x.prioridad equals p.prioridad into prioridades
                        from p in prioridades.DefaultIfEmpty()
                        where x.estado == 4 && x.id_tecnico == idSesion
                        select new TablaTicket
                        {
                            id_ticket = x.id_ticket,
                            fecha_registro_usuario = (DateTime)x.fecha_cierre_ticket,
                            tipo_incidencia = t.descripcion,
                            estado = e.descripcion,
                            prioridad = p != null ? p.descripcion : "Sin prioridad"
                        }).ToList();
            }
        }

        public List<TablaTicket> ListaHistoricoTickets()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Ticket
                        join t in context.Ticket_Tipo on x.tipo_incidencia equals t.tipo_incidencia
                        join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                        join p in context.Ticket_Prioridad on x.prioridad equals p.prioridad into prioridades
                        from p in prioridades.DefaultIfEmpty()
                        where x.estado == 4
                        select new TablaTicket
                        {
                            id_ticket = x.id_ticket,
                            fecha_registro_usuario = (DateTime)x.fecha_cierre_ticket,
                            tipo_incidencia = t.descripcion,
                            estado = e.descripcion,
                            prioridad = p != null ? p.descripcion : "Sin prioridad"
                        }).ToList();
            }
        }

        public bool DesactivarTipoIncidencia(BaseDatos.Ticket_Tipo t)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var tipo = context.Ticket_Tipo.FirstOrDefault(x => x.tipo_incidencia == t.tipo_incidencia);
                tipo.estado = false;
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }

        public bool CrearCategoria(Entidades.CategoriaIncidencia c)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var tTicket_Tipo = new BaseDatos.Ticket_Tipo();
                tTicket_Tipo.descripcion = c.descripcion;
                tTicket_Tipo.estado = true;

                context.Ticket_Tipo.Add(tTicket_Tipo);
                rowsAffected = context.SaveChanges();
            }
            return (rowsAffected > 0 ? true : false);
        }

        public BaseDatos.Ticket_Tipo verCategoria(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var query = (from x in context.Ticket_Tipo
                             where x.tipo_incidencia == id
                             select x).FirstOrDefault();

                if (query != null)
                    return query;
                else return null;
            }
        }

        public bool ActualizarCategoria(BaseDatos.Ticket_Tipo c)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {

                var ticket = context.Ticket_Tipo.FirstOrDefault(x => x.tipo_incidencia == c.tipo_incidencia);

                if (ticket != null)
                {                  
                    if (ticket.descripcion != c.descripcion & c.descripcion != null) ticket.descripcion = c.descripcion;
                }
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }

        public Entidades.Ticket reporteTecnico(Entidades.Ticket ticket)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int asignados = (from x in context.Ticket
                               join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                               where ((x.fecha_registra_tecnico >= ticket.inicioCorte && x.fecha_cierre_ticket <= ticket.finCorte) || 
                               (x.fecha_cierre_ticket == null && x.fecha_registra_tecnico <= ticket.finCorte)) && x.id_tecnico == ticket.id_tecnico
                                 select x).Count();

                int finalizados = (from x in context.Ticket
                                 where ((x.fecha_registra_tecnico >= ticket.inicioCorte && x.fecha_cierre_ticket <= ticket.finCorte) ||
                               (x.fecha_cierre_ticket == null && x.fecha_registra_tecnico <= ticket.finCorte))
                                 && x.id_tecnico == ticket.id_tecnico && x.estado == 4
                                 select x).Count();

                int pendientes = asignados - finalizados;


                var duracion = (from x in context.Ticket
                                           where x.fecha_registra_tecnico >= ticket.inicioCorte && x.fecha_cierre_ticket <= ticket.finCorte 
                                           && x.id_tecnico == ticket.id_tecnico && x.estado == 4
                                           select new {
                                               fecha_registra_tecnico = x.fecha_registra_tecnico,
                                               fecha_cierre_ticket = x.fecha_cierre_ticket
                                           }).
                                           ToList();//.Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));
                var duracion2 = duracion
    .Select(t => (t.fecha_cierre_ticket - t.fecha_registra_tecnico) ?? TimeSpan.Zero)
    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                float promedioAtencion = finalizados > 0 ? (float)(duracion2.TotalHours / finalizados) : 0;

                ticket.casosAsignados = asignados;
                ticket.casosFinalizados = finalizados;
                ticket.casosPendientes = pendientes;
                ticket.duracionMedia = promedioAtencion;
                ticket.activa = true;
                return ticket;

            }
        }

        public Entidades.Ticket reporteTecnicos(Entidades.Ticket ticket)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int asignados = (from x in context.Ticket
                                 join e in context.Ticket_Estado on x.estado equals e.estado_ticket
                                 where (x.fecha_registra_tecnico >= ticket.inicioCorte && x.fecha_cierre_ticket <= ticket.finCorte) ||
                                 (x.fecha_cierre_ticket == null && x.fecha_registra_tecnico <= ticket.finCorte)
                                 select x).Count();

                int finalizados = (from x in context.Ticket
                                   where (x.fecha_registra_tecnico >= ticket.inicioCorte && x.fecha_cierre_ticket <= ticket.finCorte) ||
                                 (x.fecha_cierre_ticket == null && x.fecha_registra_tecnico <= ticket.finCorte)
                                   && x.estado == 4
                                   select x).Count();

                int pendientes = asignados - finalizados;


                var duracion = (from x in context.Ticket
                                where x.fecha_registra_tecnico >= ticket.inicioCorte && x.fecha_cierre_ticket <= ticket.finCorte
                                           && x.estado == 4
                                select new
                                {
                                    fecha_registra_tecnico = x.fecha_registra_tecnico,
                                    fecha_cierre_ticket = x.fecha_cierre_ticket
                                }).
                                           ToList();//.Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));
                var duracion2 = duracion
    .Select(t => (t.fecha_cierre_ticket - t.fecha_registra_tecnico) ?? TimeSpan.Zero)
    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                float promedioAtencion = finalizados > 0 ? (float)(duracion2.TotalHours / finalizados) : 0;

                ticket.casosAsignados = asignados;
                ticket.casosFinalizados = finalizados;
                ticket.casosPendientes = pendientes;
                ticket.duracionMedia = promedioAtencion;
                ticket.activa = true;
                return ticket;

            }
        }

    }
}