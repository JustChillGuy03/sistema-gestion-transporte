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
│   ├── Extensions      Mapeo entidad <-> DTO, consultas de viajes y registro de dependencias
│   ├── Interfaces      IRutaRepository, IEmpleadoRepository, IVehiculoRepository, IViajeRepository
│   ├── Migrations      Migración Inicial
│   ├── Models          DTOs (Model de respuesta, AddModel y UpdateModel por entidad)
│   └── Repositories    RutaRepository, EmpleadoRepository, VehiculoRepository, ViajeRepository
└── TransporteEmpresarial.API               Web API que consume las dos capas
    ├── Controllers     Rutas, Empleados, Vehiculos, Viajes
    └── Handlers        TransporteExceptionHandler (excepciones de negocio -> 400/404/409)
```

Referencias: `API -> Infrastructure -> Domain`. La API registra la infraestructura con `AddInfrastructure(...)`.

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

Las validan los repositorios y lanzan la excepción de su entidad:

- **Capacidad:** no se inscribe un pasajero si el vehículo del viaje ya no tiene asientos libres. La inscripción corre en una transacción serializable para que dos solicitudes simultáneas no superen el cupo.
- **Horarios:** un vehículo o un conductor no puede estar en dos viajes vigentes (Programado o EnCurso) que se crucen; un pasajero tampoco. La llegada se calcula con la duración de la ruta y el turno con la hora de salida.
- **Asignaciones:** solo se asignan vehículos en estado Disponible y empleados de tipo Conductor; solo los de tipo Pasajero se inscriben.
- **Estados del viaje:** Programado → EnCurso → Completado, y Programado/EnCurso → Cancelado. Solo se reprograman viajes Programados.
- **Integridad:** no se eliminan rutas, vehículos ni empleados con viajes vigentes; un vehículo con viajes pendientes no pasa a mantenimiento ni reduce su capacidad por debajo de los inscritos.

## Endpoints

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

Errores en formato ProblemDetails: **400** datos no válidos, **404** no existe, **409** regla de negocio no cumplida.

## Cómo ejecutarlo

Requisitos: .NET SDK 10 y SQL Server LocalDB (se instala con Visual Studio).

```bash
git clone -b develop https://github.com/JustChillGuy03/sistema-gestion-transporte.git
cd sistema-gestion-transporte
dotnet run --project TransporteEmpresarial.API --launch-profile http
```

Al iniciar en modo Development se aplican las migraciones y se crea la base de datos `TransporteEmpresarialDB` con datos de ejemplo (3 rutas, 3 vehículos, 6 empleados y 3 viajes).

- Swagger UI: http://localhost:5180/swagger
- Documento OpenAPI: http://localhost:5180/openapi/v1.json
- Pruebas listas para ejecutar: `TransporteEmpresarial.API/TransporteEmpresarial.API.http` (Visual Studio o VS Code con REST Client)

La cadena de conexión está en `TransporteEmpresarial.API/appsettings.json`.

### Migraciones

```bash
dotnet tool restore
dotnet ef migrations add NombreMigracion --project TransporteEmpresarial.Infrastructure --startup-project TransporteEmpresarial.API
dotnet ef database update --project TransporteEmpresarial.Infrastructure --startup-project TransporteEmpresarial.API
```
