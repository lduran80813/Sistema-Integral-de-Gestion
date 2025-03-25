using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class ClientesModel
    {
        public bool Registro_Clientes(Entidades.Cliente cliente)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                // Se puede mejorar con procedimiento almacenado!!!!!
                //int RolSesion = int.Parse(HttpContext.Current.Session["RolUsuario"].ToString()); // Validar que el usuario que registra cliente tenga el rol de puchasing manager
                var tCliente = new BaseDatos.Venta_Cliente();

                tCliente.nombre = cliente.nombre;
                tCliente.direccion = cliente.direccion;
                tCliente.telefono = cliente.telefono;
                tCliente.correo_electronico = cliente.correo_electronico;
                tCliente.estado = 1;

                context.Venta_Cliente.Add(tCliente);
                rowsAffected = context.SaveChanges();
            }
            return (rowsAffected > 0 ? true : false);
        }

        public List<Entidades.Cliente> ListaClientes()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Venta_Cliente
                        select new Entidades.Cliente
                        {
                            id = x.id,
                            nombre = x.nombre,
                            telefono = x.telefono,
                            correo_electronico = x.correo_electronico,
                            estado = x.estado
                        }).ToList();
            }
        }

        public Entidades.Cliente ObtenerCliente(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Venta_Cliente
                        where x.id == id
                        select new Entidades.Cliente
                        {
                            id = x.id,
                            nombre = x.nombre,
                            direccion = x.direccion,
                            telefono = x.telefono,
                            correo_electronico = x.correo_electronico,
                            estado = x.estado
                        }).FirstOrDefault();
            }
        }

        public bool ActualizarCliente(Entidades.Cliente c)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {

                var cliente = context.Venta_Cliente.FirstOrDefault(x => x.id == c.id);

                if (cliente != null)
                {
                    if (cliente.nombre != c.nombre & c.nombre != null) cliente.nombre = c.nombre;

                    if (cliente.direccion != c.direccion & c.direccion != null) cliente.direccion = c.direccion;

                    if (cliente.telefono != c.telefono & c.telefono != null) cliente.telefono = c.telefono;

                    if (cliente.correo_electronico != c.correo_electronico & c.correo_electronico != null) cliente.correo_electronico = c.correo_electronico;

                    if (cliente.estado != c.estado & c.estado != 0) cliente.estado = c.estado;


                    rowsAffected = context.SaveChanges();
                }

                return (rowsAffected > 0 ? true : false);
            }
        }

        public bool EliminarCliente(Entidades.Cliente c)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var cliente = context.Venta_Cliente.FirstOrDefault(x => x.id == c.id);
                cliente.estado = 0;
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }
    }
}