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
                return (from x in context.Empleado
                        where x.rol_id == 2 //&& x.estado_usuario == 1 //añadir estado a usuario
                        select new UsuarioEmpleado 
                        {
                            id_usuario = x.id,
                            nombre_completo = x.nombre + " " + x.apellidos
                         }).ToList();
            }
        }


        public List<UsuarioEmpleado> ConsultarUsuarios()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Empleado
                        //where x.estado_usuario == 1 //añadir estado a usuario
                        select new UsuarioEmpleado
                        {
                            id_usuario = x.id,
                            nombre_completo = x.nombre + " " + x.apellidos
                        }).ToList();
            }
        }

        public List<ConsultarClientes_Result> ConsultarClientes()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ConsultarClientes().ToList();
            }
        }

        public List<listaMetodoPago_Result> ConsultarMetodoPago()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.listaMetodoPago().ToList();
            }
        }

        public List<listaTipoVenta_Result> ConsultarTipoVenta()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.listaTipoVenta().ToList();
            }
        }
        public List<ListaCuentasContables_Result> ConsultarCuentasContables()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ListaCuentasContables().ToList();
            }
        }
        public List<ListaEntidadesFinancieras_Result> ConsultarEntidadesFinancieras()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ListaEntidadesFinancieras().ToList();
            }
        }

    }
}