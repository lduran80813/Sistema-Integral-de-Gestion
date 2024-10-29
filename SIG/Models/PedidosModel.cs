using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIG.Models
{
    public class PedidosModel
    {
        public bool RegistrarPedido(Entidades.Pedido pedido)
        {
            int rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var nuevoPedido = new BaseDatos.Pedido
                {
                    fecha_pedido = pedido.fecha_pedido,
                    proveedor_id = pedido.proveedor_id,
                    total_pedido = pedido.total_pedido,
                    EstadoCompraId = pedido.EstadoCompraId
                };

                context.Pedido.Add(nuevoPedido);
                rowsAffected = context.SaveChanges();

                if (rowsAffected > 0)
                {
                    var historialPedido = new BaseDatos.HistorialPedido
                    {
                        PedidoId = nuevoPedido.id,
                        DetalleCambio = "Pedido registrado",
                        EstadoCompraId = nuevoPedido.EstadoCompraId
                    };
                    RegistrarHistorialPedido(historialPedido);
                }
            }

            return rowsAffected > 0;
        }

        public bool EsProveedorActivo(int proveedorId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var proveedor = context.Proveedor.FirstOrDefault(p => p.id == proveedorId);
                return proveedor != null && proveedor.estado == 1;
            }
        }

        public List<Entidades.Pedido> ListaPedidos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from p in context.Pedido
                        select new Entidades.Pedido
                        {
                            id = p.id,
                            fecha_pedido = p.fecha_pedido,
                            proveedor_id = p.proveedor_id,
                            total_pedido = (int)p.total_pedido,
                            EstadoCompraId = p.EstadoCompraId
                        }).ToList();
            }
        }

        public Entidades.Pedido ObtenerPedidoPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from p in context.Pedido
                        where p.id == id
                        select new Entidades.Pedido
                        {
                            id = p.id,
                            fecha_pedido = p.fecha_pedido,
                            proveedor_id = p.proveedor_id,
                            total_pedido = (int)p.total_pedido,
                            EstadoCompraId = p.EstadoCompraId
                        }).FirstOrDefault();
            }
        }

        public bool ModificarPedido(Entidades.Pedido pedido)
        {
            int rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var pedidoExistente = context.Pedido.FirstOrDefault(p => p.id == pedido.id);
                if (pedidoExistente != null)
                {
                    pedidoExistente.fecha_pedido = pedido.fecha_pedido;
                    pedidoExistente.proveedor_id = pedido.proveedor_id;
                    pedidoExistente.total_pedido = pedido.total_pedido;
                    pedidoExistente.EstadoCompraId = pedido.EstadoCompraId;

                    rowsAffected = context.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        var historialModificacion = new BaseDatos.HistorialModificacion
                        {
                            PedidoId = pedidoExistente.id,
                            DetallesModificacion = "Detalles del pedido modificados"
                        };
                        RegistrarHistorialModificacion(historialModificacion);
                    }
                }
            }

            return rowsAffected > 0;
        }

        public bool EliminarPedido(int id)
        {
            return false; // Implementar lógica si es necesario.
        }

        public List<Entidades.HistorialPedido> ObtenerHistorialPedido(int pedidoId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.HistorialPedido
                    .Where(h => h.PedidoId == pedidoId)
                    .Select(h => new Entidades.HistorialPedido
                    {
                        PedidoId = h.PedidoId,
                        FechaModificacion = h.FechaModificacion,
                        DetalleCambio = h.DetalleCambio,
                        EstadoCompraId = h.EstadoCompraId
                    })
                    .ToList();
            }
        }

        private void RegistrarHistorialPedido(BaseDatos.HistorialPedido historialPedido)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                historialPedido.FechaModificacion = DateTime.Now;
                context.HistorialPedido.Add(historialPedido);
                context.SaveChanges();
            }
        }

        private void RegistrarHistorialModificacion(BaseDatos.HistorialModificacion historialModificacion)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                historialModificacion.FechaModificacion = DateTime.Now;
                context.HistorialModificacion.Add(historialModificacion);
                context.SaveChanges();
            }
        }

        public List<BaseDatos.EstadoCompra> ListaEstadosCompra()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.EstadoCompra.ToList();
            }
        }

        public bool CancelarPedido(int id)
        {
            int rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                var pedido = context.Pedido.FirstOrDefault(p => p.id == id);
                if (pedido != null)
                {
                    pedido.EstadoCompraId = 4; // Asumiendo que 4 es el ID del estado cancelado.
                    rowsAffected = context.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        var historialPedido = new BaseDatos.HistorialPedido
                        {
                            PedidoId = pedido.id,
                            DetalleCambio = "Pedido cancelado",
                            EstadoCompraId = pedido.EstadoCompraId
                        };
                        RegistrarHistorialPedido(historialPedido);
                    }
                }
            }
            return rowsAffected > 0;
        }

        public bool EsEstadoCompraValido(int estadoId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.EstadoCompra.Any(e => e.Id == estadoId);
            }
        }
        public List<Entidades.ReportePedidos> GenerarReporte(int proveedorId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Pedido
                    .Where(p => p.proveedor_id == proveedorId)
                    .Select(p => new Entidades.ReportePedidos
                    {
                        PedidoId = p.id,
                        FechaPedido = p.fecha_pedido,
                        Proveedor = p.Proveedor.nombre,
                        Total = p.total_pedido,
                        Evaluacion = p.EstadoCompra.Nombre
                    })
                    .ToList();
            }
        }
    }
}
