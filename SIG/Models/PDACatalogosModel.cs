using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIG.Models
{
    public class PDACatalogosModel
    {
        // Consulta los responsables para asignarlos a acciones correctivas o preventivas (Historia de Usuario: PDA-001, Paso 1).
        public List<UsuarioEmpleado> ConsultarResponsables()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Empleado.Select(e => new UsuarioEmpleado
                {
                    id_usuario = e.id,
                    nombre_completo = e.nombre + " " + e.apellidos
                }).ToList();
            }
        }

        // Obtiene información de un responsable específico para mostrar detalles o asignar a una acción (Historia de Usuario: PDA-001, Paso 1).
        public UsuarioEmpleado ConsultarResponsablePorId(int responsableId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Empleado
                    .Where(e => e.id == responsableId)
                    .Select(e => new UsuarioEmpleado
                    {
                        id_usuario = e.id,
                        nombre_completo = e.nombre + " " + e.apellidos
                    }).FirstOrDefault();
            }
        }

        // Consulta los tipos de acción disponibles para crear una acción correctiva o preventiva (Historia de Usuario: PDA-001, Paso 1).
        public List<Entidades.PDATipoAccion> ConsultarTiposAccion()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.PDA_TipoAccion.Select(ta => new Entidades.PDATipoAccion
                {
                    Id = ta.Id,            
                    Descripcion = ta.Descripcion   
                }).ToList();
            }
        }

        // Obtiene los estados posibles de una acción (creada, validada, rechazada, etc.) para registrar o gestionar cambios (Historia de Usuario: PDA-001, Paso 3, Paso 4 y Paso 5).
        public List<Entidades.PDAEstado> ConsultarEstadosAccion()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.PDA_Estado.Select(ea => new Entidades.PDAEstado
                {
                    Id = ea.id,                          
                    NombreEstado = ea.descripcion_estado, 
                    Descripcion = ea.descripcion_estado  
                }).ToList();
            }
        }
    }
}
