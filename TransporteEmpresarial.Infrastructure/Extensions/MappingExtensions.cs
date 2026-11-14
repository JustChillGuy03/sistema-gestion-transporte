using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Extensions
{
    /// <summary>
    /// Conversión de entidades a los modelos de respuesta (Models).
    /// La conversión de los DTOs de entrada a entidades la hace la capa de servicios (Application).
    /// </summary>
    public static class MappingExtensions
    {
        public static VehiculoModel ToModel(this Vehiculo v) => new()
        {
            Id = v.Id,
            Placa = v.Placa,
            Marca = v.Marca,
            Modelo = v.Modelo,
            Anio = v.Anio,
            Capacidad = v.Capacidad,
            Estado = v.Estado
        };

        public static EmpleadoModel ToModel(this Empleado e) => new()
        {
            Id = e.Id,
            Cedula = e.Cedula,
            Nombre = e.Nombre,
            Apellido = e.Apellido,
            NombreCompleto = e.NombreCompleto,
            Telefono = e.Telefono,
            Correo = e.Correo,
            Tipo = e.Tipo,
            Departamento = e.Departamento,
            PuntoRecogida = e.PuntoRecogida,
            LicenciaConducir = e.LicenciaConducir
        };

        public static RutaModel ToModel(this Ruta r) => new()
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Origen = r.Origen,
            Destino = r.Destino,
            ParadasIntermedias = r.ParadasIntermedias.ToList(),
            DuracionEstimadaMinutos = r.DuracionEstimadaMinutos
        };
    }
}
