using Microsoft.EntityFrameworkCore;
using TransporteEmpresarial.Domain.Entities;
using TransporteEmpresarial.Domain.Enums;

namespace TransporteEmpresarial.Infrastructure.Context
{
    /// <summary>
    /// Datos de ejemplo para probar la API apenas se crea la base de datos.
    /// Las fechas son fijas para que la migración no cambie cada vez que se genera.
    /// </summary>
    internal static class DatosIniciales
    {
        private static readonly DateTime FechaCarga = new(2026, 11, 14);

        public static void Cargar(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ruta>().HasData(
                new Ruta
                {
                    Id = 1, Nombre = "Ruta Norte - Villa Mella", Origen = "Villa Mella, Santo Domingo Norte",
                    Destino = "Sede Central, Av. Winston Churchill",
                    ParadasIntermedias = ["Los Guaricanos", "Av. Charles de Gaulle", "Puente de la 17"],
                    DuracionEstimadaMinutos = 60, FechaCreacion = FechaCarga
                },
                new Ruta
                {
                    Id = 2, Nombre = "Ruta Este - Los Mina", Origen = "Los Mina, Santo Domingo Este",
                    Destino = "Sede Central, Av. Winston Churchill",
                    ParadasIntermedias = ["Av. San Vicente de Paul", "Puente Juan Bosch"],
                    DuracionEstimadaMinutos = 45, FechaCreacion = FechaCarga
                },
                new Ruta
                {
                    Id = 3, Nombre = "Ruta Oeste - Herrera", Origen = "Herrera, Santo Domingo Oeste",
                    Destino = "Sede Central, Av. Winston Churchill",
                    ParadasIntermedias = ["Av. Isabel Aguiar", "Av. 27 de Febrero"],
                    DuracionEstimadaMinutos = 40, FechaCreacion = FechaCarga
                }
            );

            modelBuilder.Entity<Vehiculo>().HasData(
                new Vehiculo { Id = 1, Placa = "I000101", Marca = "Toyota", Modelo = "Hiace", Anio = 2022, Capacidad = 14, Estado = EstadoVehiculo.Disponible, FechaCreacion = FechaCarga },
                new Vehiculo { Id = 2, Placa = "I000102", Marca = "Hyundai", Modelo = "County", Anio = 2021, Capacidad = 25, Estado = EstadoVehiculo.Disponible, FechaCreacion = FechaCarga },
                new Vehiculo { Id = 3, Placa = "I000103", Marca = "Nissan", Modelo = "Urvan", Anio = 2019, Capacidad = 12, Estado = EstadoVehiculo.EnMantenimiento, FechaCreacion = FechaCarga }
            );

            modelBuilder.Entity<Empleado>().HasData(
                new Empleado { Id = 1, Cedula = "001-0000001-1", Nombre = "Juan", Apellido = "Pérez", Telefono = "809-555-0101", Tipo = TipoEmpleado.Conductor, Departamento = "Transporte", LicenciaConducir = "LIC-0001", FechaCreacion = FechaCarga },
                new Empleado { Id = 2, Cedula = "001-0000002-2", Nombre = "María", Apellido = "Rodríguez", Telefono = "829-555-0202", Tipo = TipoEmpleado.Conductor, Departamento = "Transporte", LicenciaConducir = "LIC-0002", FechaCreacion = FechaCarga },
                new Empleado { Id = 3, Cedula = "001-0000003-3", Nombre = "Carlos", Apellido = "Gómez", Correo = "cgomez@empresa.com", Tipo = TipoEmpleado.Pasajero, Departamento = "Contabilidad", PuntoRecogida = "Parada Los Guaricanos", FechaCreacion = FechaCarga },
                new Empleado { Id = 4, Cedula = "001-0000004-4", Nombre = "Ana", Apellido = "Martínez", Correo = "amartinez@empresa.com", Tipo = TipoEmpleado.Pasajero, Departamento = "Recursos Humanos", PuntoRecogida = "Av. Charles de Gaulle", FechaCreacion = FechaCarga },
                new Empleado { Id = 5, Cedula = "001-0000005-5", Nombre = "Luis", Apellido = "Fernández", Correo = "lfernandez@empresa.com", Tipo = TipoEmpleado.Pasajero, Departamento = "Tecnología", PuntoRecogida = "Puente Juan Bosch", FechaCreacion = FechaCarga },
                new Empleado { Id = 6, Cedula = "001-0000006-6", Nombre = "Laura", Apellido = "Santos", Correo = "lsantos@empresa.com", Tipo = TipoEmpleado.Pasajero, Departamento = "Ventas", PuntoRecogida = "Av. 27 de Febrero", FechaCreacion = FechaCarga }
            );

            modelBuilder.Entity<Viaje>().HasData(
                // Viaje ya realizado: sirve para ver el historial de traslados
                new Viaje
                {
                    Id = 1, RutaId = 1, VehiculoId = 1, ConductorId = 1,
                    FechaHoraSalida = new DateTime(2026, 11, 10, 7, 0, 0), FechaHoraLlegada = new DateTime(2026, 11, 10, 8, 0, 0),
                    Turno = Turno.Manana, Estado = EstadoViaje.Completado, FechaCreacion = FechaCarga
                },
                new Viaje
                {
                    Id = 2, RutaId = 1, VehiculoId = 1, ConductorId = 1,
                    FechaHoraSalida = new DateTime(2026, 12, 1, 7, 0, 0), FechaHoraLlegada = new DateTime(2026, 12, 1, 8, 0, 0),
                    Turno = Turno.Manana, Estado = EstadoViaje.Programado, FechaCreacion = FechaCarga
                },
                new Viaje
                {
                    Id = 3, RutaId = 2, VehiculoId = 2, ConductorId = 2,
                    FechaHoraSalida = new DateTime(2026, 12, 1, 7, 0, 0), FechaHoraLlegada = new DateTime(2026, 12, 1, 7, 45, 0),
                    Turno = Turno.Manana, Estado = EstadoViaje.Programado, FechaCreacion = FechaCarga
                }
            );

            modelBuilder.Entity<ViajePasajero>().HasData(
                new ViajePasajero { ViajeId = 1, EmpleadoId = 3, FechaInscripcion = new DateTime(2026, 11, 9) },
                new ViajePasajero { ViajeId = 1, EmpleadoId = 4, FechaInscripcion = new DateTime(2026, 11, 9) },
                new ViajePasajero { ViajeId = 2, EmpleadoId = 3, FechaInscripcion = FechaCarga },
                new ViajePasajero { ViajeId = 2, EmpleadoId = 4, FechaInscripcion = FechaCarga },
                new ViajePasajero { ViajeId = 3, EmpleadoId = 5, FechaInscripcion = FechaCarga }
            );
        }
    }
}
