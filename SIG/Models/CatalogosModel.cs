using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class CatalogosModel
    {
        public List<Ticket_Tipo> ConsultarTicketTipo()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Ticket_Tipo
                        select x).ToList();
            }
        }

        public List<Ticket_Estado> ConsultarEstados()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Ticket_Estado
                        select x).ToList();
            }
        }
        public List<Ticket_Prioridad> ConsultarPrioridades()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Ticket_Prioridad
                        select x).ToList();
            }
        }
        public List<UsuarioEmpleado> ConsultarTecnicos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Usuario
                        join y in context.Empleado on x.empleado_id equals y.id
                        where x.rol_id == 2 //&& x.estado_usuario == 1 //añadir estado a usuario
                        select new UsuarioEmpleado 
                        {
                            id_usuario = x.id,
                            nombre_completo = y.nombre + " " + y.apellidos
                         }).ToList();
            }
        }


        public List<UsuarioEmpleado> ConsultarUsuarios()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Usuario
                        join y in context.Empleado on x.empleado_id equals y.id
                        //where x.estado_usuario == 1 //añadir estado a usuario
                        select new UsuarioEmpleado
                        {
                            id_usuario = x.id,
                            nombre_completo = y.nombre + " " + y.apellidos
                        }).ToList();
            }
        }

    }
}