using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;
using TransporteEmpresarial.Infrastructure.Models;

namespace TransporteEmpresarial.Infrastructure.Extensions
{
    /// <summary>
    /// Conversión entre entidades y DTOs (Models).
    /// </summary>
    public static class MappingExtensions
    {
        // ---------- Vehículo ----------
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

        public static Vehiculo ToEntity(this VehiculoAddModel m) => new()
        {
            Placa = NormalizarPlaca(m.Placa),
            Marca = m.Marca.Trim(),
            Modelo = m.Modelo.Trim(),
            Anio = m.Anio,
            Capacidad = m.Capacidad,
            Estado = EstadoVehiculo.Disponible
        };

        public static void ActualizarDesde(this Vehiculo v, VehiculoUpdateModel m)
        {
            v.Placa = NormalizarPlaca(m.Placa);
            v.Marca = m.Marca.Trim();
            v.Modelo = m.Modelo.Trim();
            v.Anio = m.Anio;
            v.Capacidad = m.Capacidad;
            v.Estado = m.Estado!.Value;
        }

        // ---------- Empleado ----------
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

        public static Empleado ToEntity(this EmpleadoAddModel m)
        {
            var empleado = new Empleado { Tipo = m.Tipo!.Value };
            empleado.ActualizarDesde(m);
            return empleado;
        }

        public static void ActualizarDesde(this Empleado e, EmpleadoUpdateModel m)
        {
            e.Cedula = m.Cedula.Trim();
            e.Nombre = m.Nombre.Trim();
            e.Apellido = m.Apellido.Trim();
            e.Telefono = Limpiar(m.Telefono);
            e.Correo = Limpiar(m.Correo);
            e.Departamento = Limpiar(m.Departamento);
            e.PuntoRecogida = Limpiar(m.PuntoRecogida);
            e.LicenciaConducir = Limpiar(m.LicenciaConducir);
        }

        // ---------- Ruta ----------
        public static RutaModel ToModel(this Ruta r) => new()
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Origen = r.Origen,
            Destino = r.Destino,
            ParadasIntermedias = r.ParadasIntermedias.ToList(),
            DuracionEstimadaMinutos = r.DuracionEstimadaMinutos
        };

        public static Ruta ToEntity(this RutaAddModel m)
        {
            var ruta = new Ruta();
            ruta.ActualizarDesde(m);
            return ruta;
        }

        public static void ActualizarDesde(this Ruta r, RutaAddModel m)
        {
            r.Nombre = m.Nombre.Trim();
            r.Origen = m.Origen.Trim();
            r.Destino = m.Destino.Trim();
            r.DuracionEstimadaMinutos = m.DuracionEstimadaMinutos;

            // Se descartan paradas vacías y espacios sobrantes
            r.ParadasIntermedias = (m.ParadasIntermedias ?? new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .ToList();
        }

        // ---------- Viaje ----------
        public static Viaje ToEntity(this ViajeAddModel m)
        {
            var viaje = new Viaje();
            viaje.ActualizarDesde(m);
            return viaje;
        }

        // La llegada y el turno los completa el repositorio al validar el viaje
        public static void ActualizarDesde(this Viaje v, ViajeAddModel m)
        {
            v.RutaId = m.RutaId;
            v.VehiculoId = m.VehiculoId;
            v.ConductorId = m.ConductorId;
            v.FechaHoraSalida = m.FechaHoraSalida!.Value;
        }

        private static string NormalizarPlaca(string placa) => placa.Trim().ToUpperInvariant();

        private static string? Limpiar(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
