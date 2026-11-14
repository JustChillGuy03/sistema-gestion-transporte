# Sistema de Gestión de Transporte Empresarial

API REST en ASP.NET Core para administrar el transporte del personal de una empresa: rutas, empleados (pasajeros y conductores), vehículos y viajes. Controla el cupo de cada vehículo, evita que un vehículo o un conductor queden asignados a dos viajes al mismo tiempo y guarda el historial de traslados de cada empleado.

## Tecnologías

- .NET 10 / ASP.NET Core Web API (controladores)
- Entity Framework Core 10 con SQL Server (LocalDB)
- OpenAPI + Swagger UI

## Estructura de la solución

```
TransporteEmpresarial.slnx
├── TransporteEmpresarial.Domain            Capa de dominio (sin dependencias externas)
│   ├── Core            BaseEntity (Id, auditoría, borrado lógico), Persona
│   ├── Entities        Ruta, Empleado, Vehiculo, Viaje, ViajePasajero
│   ├── Enums           EstadoVehiculo, TipoEmpleado, EstadoViaje, Turno
│   └── Repository      IBaseRepository<T>
├── TransporteEmpresarial.Infrastructure    Acceso a datos
│   ├── Context         TransporteContext, configuraciones Fluent API y datos iniciales
│   ├── Core            BaseRepository<T>
│   ├── Exceptions      RutaException, EmpleadoException, VehiculoException, ViajeException
│   ├── Extensions      Mapeo entidad -> modelo de respuesta, consultas de viajes y registro de dependencias
│   ├── Interfaces      IRutaRepository, IEmpleadoRepository, IVehiculoRepository, IViajeRepository
│   ├── Migrations      Migración Inicial
│   ├── Models          Modelos de respuesta (RutaModel, EmpleadoModel, VehiculoModel, ViajeModel...)
│   └── Repositories    RutaRepository, EmpleadoRepository, VehiculoRepository, ViajeRepository
├── TransporteEmpresarial.Application       Capa de servicios (lógica de negocio y validaciones)
│   ├── Contract        IRutaService, IEmpleadoService, IVehiculoService, IViajeService
│   ├── Core            ServiceResult, IBaseService, BaseService, Validador, ServiceErrorType, NombreEntidad
│   ├── Dtos            DtoBase y, por entidad (Ruta, Empleado, Vehiculo, Viaje): DtoBase, DtoAdd y DtoUpdate
│   ├── Exceptions      ServiceException (hereda de Exception) y una por servicio
│   └── Service         RutaService, EmpleadoService, VehiculoService, ViajeService
└── TransporteEmpresarial.API               Web API que consume la capa de servicios
    ├── Controllers     Rutas, Empleados, Vehiculos, Viajes
    └── Handlers        TransporteExceptionHandler (respaldo: excepciones de negocio -> 400/404/409)
```

Referencias: `API -> Application -> Infrastructure -> Domain`. La API registra la infraestructura con `AddInfrastructure(...)` y los servicios en `Program.cs`.

## Capa de servicios

Los controladores solo llaman a los servicios. Cada operación del servicio sigue los mismos pasos (definidos en `BaseService`):

1. **Normaliza** el DTO: quita espacios sobrantes, pone la placa y la licencia en mayúsculas, da formato a la cédula (`00100000011` -> `001-0000001-1`).
2. **Valida cada campo** y reúne todos los errores, agrupados por campo.
3. Convierte el DTO en entidad y llama al **repositorio**, que aplica las reglas que dependen de la base de datos.
4. Devuelve un **`ServiceResult`** (`Success`, `Message`, `Data`, `ErrorType`, `Errors`). Si algo falla, el servicio captura la excepción (de validación, del repositorio o inesperada) y la convierte en un resultado fallido; la API lo traduce a 400, 404, 409 o 500.

### Validaciones por campo

| Entidad | Campo | Validación |
|---|---|---|
| Ruta | Nombre | Obligatorio, 3 a 100 caracteres (único) |
| | Origen / Destino | Obligatorios, 3 a 150 caracteres, el destino distinto del origen |
| | ParadasIntermedias | Opcional; máximo 20, cada una de 3 a 150 caracteres, sin vacías ni repetidas, distintas del origen y el destino |
| | DuracionEstimadaMinutos | Obligatoria, de 5 a 600 |
| Vehiculo | Placa | Obligatoria, 5 a 10 caracteres, letras, números y guiones, con al menos una letra y un número (única) |
| | Marca | Obligatoria, 2 a 50 caracteres |
| | Modelo | Obligatorio, 1 a 50 caracteres |
| | Anio | Obligatorio, de 1990 al año próximo |
| | Capacidad | Obligatoria, de 1 a 60 asientos |
| | Estado (al actualizar) | Obligatorio: Disponible, EnMantenimiento o FueraDeServicio |
| Empleado | Cedula | Obligatoria, formato `000-0000000-0` (única) |
| | Nombre / Apellido | Obligatorios, 2 a 60 caracteres, solo letras, espacios, apóstrofes, puntos y guiones |
| | Telefono | Opcional; 7 a 20 caracteres con 7 a 15 dígitos |
| | Correo | Opcional; formato `usuario@dominio.com`, máximo 100 caracteres |
| | Tipo (al registrar) | Obligatorio: Pasajero o Conductor (no cambia al actualizar) |
| | Departamento | Opcional; 2 a 60 caracteres |
| | PuntoRecogida | Obligatorio para pasajeros (3 a 150 caracteres); no se registra para conductores |
| | LicenciaConducir | Obligatoria para conductores (5 a 20 caracteres, letras, números y guiones); no se registra para pasajeros |
| Viaje | RutaId / VehiculoId / ConductorId | Obligatorios y mayores que cero |
| | FechaHoraSalida | Obligatoria, futura y con máximo 12 meses de anticipación |
| | Estado (al cambiarlo) | Obligatorio: EnCurso, Completado o Cancelado |


## Modelo de dominio

| Entidad | Datos principales |
|---|---|
| **Vehiculo** | Placa (única), marca, modelo, año, capacidad de asientos, estado (Disponible, EnMantenimiento, FueraDeServicio) |
| **Empleado** | Cédula (única), nombre, apellido, teléfono, correo, tipo (Pasajero o Conductor), departamento, punto de recogida, licencia (obligatoria para conductores) |
| **Ruta** | Nombre (único), origen, destino, paradas intermedias, duración estimada en minutos |
| **Viaje** | Ruta + vehículo + conductor, salida, llegada estimada, turno, estado (Programado, EnCurso, Completado, Cancelado) |
| **ViajePasajero** | Inscripción de un pasajero en un viaje (lista de pasajeros e historial de traslados) |

Todas las entidades heredan de `BaseEntity`: el borrado es lógico (`Eliminado`) para no perder el historial.

## Reglas de negocio

Las que dependen de los datos guardados las validan los repositorios, que lanzan la excepción de su entidad (el servicio la convierte en un `ServiceResult`):

- **Capacidad:** no se inscribe un pasajero si el vehículo del viaje ya no tiene asientos libres. La inscripción corre en una transacción serializable para que dos solicitudes simultáneas no superen el cupo.
- **Horarios:** un vehículo o un conductor no puede estar en dos viajes vigentes (Programado o EnCurso) que se crucen; un pasajero tampoco. La llegada se calcula con la duración de la ruta y el turno con la hora de salida.
- **Asignaciones:** solo se asignan vehículos en estado Disponible y empleados de tipo Conductor; solo los de tipo Pasajero se inscriben.
- **Estados del viaje:** Programado → EnCurso → Completado, y Programado/EnCurso → Cancelado. Solo se reprograman viajes Programados.
- **Integridad:** no se eliminan rutas, vehículos ni empleados con viajes vigentes; un vehículo con viajes pendientes no pasa a mantenimiento ni reduce su capacidad por debajo de los inscritos.

## Rutas de la API

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/rutas` | Lista de rutas |
| GET | `/api/rutas/{id}` | Una ruta |
| GET | `/api/rutas/{id}/viajes` | Viajes de la ruta con vehículo y conductor asignados |
| POST / PUT / DELETE | `/api/rutas`, `/api/rutas/{id}` | Crear, actualizar, eliminar |
| GET | `/api/vehiculos` | Lista de vehículos |
| GET | `/api/vehiculos/{id}` | Un vehículo |
| GET | `/api/vehiculos/disponibles?desde=&hasta=` | Vehículos libres en un horario |
| POST / PUT / DELETE | `/api/vehiculos`, `/api/vehiculos/{id}` | Crear, actualizar (incluye estado), eliminar |
| GET | `/api/empleados?tipo=Conductor` | Lista de empleados (filtro opcional por tipo) |
| GET | `/api/empleados/{id}` | Un empleado |
| GET | `/api/empleados/{id}/historial` | Traslados como pasajero y viajes como conductor |
| POST / PUT / DELETE | `/api/empleados`, `/api/empleados/{id}` | Crear, actualizar, eliminar |
| GET | `/api/viajes?fecha=&estado=` | Lista de viajes (filtros opcionales) |
| GET | `/api/viajes/{id}` | Un viaje con cupos disponibles |
| POST | `/api/viajes` | Programar un viaje |
| PUT | `/api/viajes/{id}` | Reprogramar un viaje |
| PATCH | `/api/viajes/{id}/estado` | Cambiar el estado |
| DELETE | `/api/viajes/{id}` | Eliminar (solo Programado o Cancelado) |
| GET | `/api/viajes/{id}/pasajeros` | Pasajeros inscritos |
| POST | `/api/viajes/{id}/pasajeros/{empleadoId}` | Inscribir un pasajero |
| DELETE | `/api/viajes/{id}/pasajeros/{empleadoId}` | Quitar un pasajero |


### Migraciones

```bash
dotnet tool restore
dotnet ef migrations add NombreMigracion --project TransporteEmpresarial.Infrastructure --startup-project TransporteEmpresarial.API
dotnet ef database update --project TransporteEmpresarial.Infrastructure --startup-project TransporteEmpresarial.API
```
