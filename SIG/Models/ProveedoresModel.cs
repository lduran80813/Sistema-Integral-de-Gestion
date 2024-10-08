using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace SIG.Models
{
    public class ProveedoresModel
    {
        public bool Registro_Proveedores(Entidades.Proveedor proveedor)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                // Se puede mejorar con procedimiento almacenado!!!!!
                //int RolSesion = int.Parse(HttpContext.Current.Session["RolUsuario"].ToString()); // Validar que el usuario que registra proveedor tenga el rol de puchasing manager
                var tProveedor = new BaseDatos.Proveedor();

                tProveedor.nombre = proveedor.nombre;
                tProveedor.direccion = proveedor.direccion;
                tProveedor.estado = 1;

                context.Proveedor.Add(tProveedor);
                rowsAffected = context.SaveChanges();

                if (rowsAffected > 0)
                {
                    var idProveedor = int.Parse((from x in context.Proveedor
                                                 orderby x.id descending
                                                 select x.id).FirstOrDefault().ToString());
                    var tProvContacto = new BaseDatos.Prov_Contacto();
                    tProvContacto.proveedor_id = idProveedor;
                    tProvContacto.nombre_contacto = proveedor.nombre_contacto;
                    tProvContacto.telefono = proveedor.telefono;
                    tProvContacto.correo_electronico = proveedor.correo_electronico;
                    tProvContacto.descripcion_adicional = proveedor.descripcion_adicional;

                    context.Prov_Contacto.Add(tProvContacto);
                    rowsAffected = context.SaveChanges();
                }
            }
            return (rowsAffected > 0 ? true : false);
        }

        public List<Entidades.Proveedor> ListaProveedores()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Proveedor
                        join d in context.Prov_Contacto on x.id equals d.proveedor_id
                        //Si se considera un estado de proveedor aquí se podría añadir un filtro para los que están activos
                        select new Entidades.Proveedor
                        {
                            id = x.id,
                            nombre = x.nombre,
                            estado = x.estado,
                            telefono = d.telefono,
                            correo_electronico = d.correo_electronico,
                            nombre_contacto = d.nombre_contacto
                        }).ToList();
            }
        }

        public Entidades.Proveedor ObtenerProveedor(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Proveedor
                        join d in context.Prov_Contacto on x.id equals d.proveedor_id
                        where x.id == id
                        select new Entidades.Proveedor
                        {
                            id = x.id,
                            nombre = x.nombre,
                            direccion = x.direccion,
                            estado = x.estado,
                            nombre_contacto = d.nombre_contacto,
                            telefono = d.telefono,
                            correo_electronico = d.correo_electronico,
                            descripcion_adicional = d.descripcion_adicional
                        }).FirstOrDefault();
            }
        }

        public bool ActualizarProveedor(Entidades.Proveedor p)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {

                var proveedor = context.Proveedor.FirstOrDefault(x => x.id == p.id);
                var provContacto = context.Prov_Contacto.FirstOrDefault(x => x.proveedor_id == p.id);

                if (proveedor != null)
                {
                    if (proveedor.nombre != p.nombre & p.nombre != null) proveedor.nombre = p.nombre;

                    if (proveedor.direccion != p.direccion & p.direccion != null) proveedor.direccion = p.direccion;

                    if (proveedor.estado != p.estado & p.estado != 0) proveedor.estado = p.estado;

                    rowsAffected = context.SaveChanges();

                    if (provContacto != null)
                    {
                        if (provContacto.nombre_contacto != p.nombre_contacto & p.nombre_contacto != null) provContacto.nombre_contacto = p.nombre_contacto;

                        if (provContacto.telefono != p.telefono & p.telefono != null) provContacto.telefono = p.telefono;

                        if (provContacto.correo_electronico != p.correo_electronico & p.correo_electronico != null) provContacto.correo_electronico = p.correo_electronico;

                        if (provContacto.descripcion_adicional != p.descripcion_adicional & p.descripcion_adicional != null) provContacto.descripcion_adicional = p.descripcion_adicional;
                    }
                }
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }

        public bool EliminarProveedor(Entidades.Proveedor p)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var proveedor = context.Proveedor.FirstOrDefault(x => x.id == p.id);
                proveedor.estado = 0;
                rowsAffected = context.SaveChanges();
            }

            return (rowsAffected > 0 ? true : false);
        }

        public EstadoFinanciero ObtenerEstadoFinanciero(int proveedorId)
        {
            try
            {
                using (var context = new SistemaIntegralGestionEntities())
                {
                    var estadoFinanciero = context.EstadosFinancieros
                        .FirstOrDefault(e => e.ProveedorId == proveedorId);

                    if (estadoFinanciero != null)
                    {
                        return new EstadoFinanciero
                        {
                            MensajeError = null,
                        };
                    }

                    return new EstadoFinanciero { MensajeError = "Proveedor no encontrado." };
                }
            }
            catch (Exception)
            {
                return new EstadoFinanciero { MensajeError = "Error en la conexión a la base de datos." };
            }
        }


        public List<EstadoFinanciero> ObtenerEstadoFinancieroConjunto(List<int> proveedoresIds)
        {
            try
            {
                using (var context = new SistemaIntegralGestionEntities())
                {
                    var estadosFinancieros = context.EstadosFinancieros
                        .Where(e => proveedoresIds.Contains(e.ProveedorId))
                        .ToList();

                    var result = estadosFinancieros.Select(e => new EstadoFinanciero
                    {
                        MensajeError = null,

                    }).ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return new List<EstadoFinanciero> { new EstadoFinanciero { MensajeError = "Error en la conexión a la base de datos." } };
            }
        }
    }
}