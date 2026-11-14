using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TransporteEmpresarial.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PuntoRecogida = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LicenciaConducir = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cedula = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rutas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Origen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Destino = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ParadasIntermedias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuracionEstimadaMinutos = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rutas", x => x.Id);
                    table.CheckConstraint("CK_Rutas_Duracion", "[DuracionEstimadaMinutos] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Placa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                    table.CheckConstraint("CK_Vehiculos_Capacidad", "[Capacidad] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Viajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RutaId = table.Column<int>(type: "int", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    ConductorId = table.Column<int>(type: "int", nullable: false),
                    FechaHoraSalida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraLlegada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Turno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viajes", x => x.Id);
                    table.CheckConstraint("CK_Viajes_Horario", "[FechaHoraLlegada] > [FechaHoraSalida]");
                    table.ForeignKey(
                        name: "FK_Viajes_Empleados_ConductorId",
                        column: x => x.ConductorId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Viajes_Rutas_RutaId",
                        column: x => x.RutaId,
                        principalTable: "Rutas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Viajes_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ViajePasajeros",
                columns: table => new
                {
                    ViajeId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViajePasajeros", x => new { x.ViajeId, x.EmpleadoId });
                    table.ForeignKey(
                        name: "FK_ViajePasajeros_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViajePasajeros_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Empleados",
                columns: new[] { "Id", "Apellido", "Cedula", "Correo", "Departamento", "Eliminado", "FechaCreacion", "FechaEliminacion", "FechaModificacion", "LicenciaConducir", "Nombre", "PuntoRecogida", "Telefono", "Tipo" },
                values: new object[,]
                {
                    { 1, "Pérez", "001-0000001-1", null, "Transporte", false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "LIC-0001", "Juan", null, "809-555-0101", "Conductor" },
                    { 2, "Rodríguez", "001-0000002-2", null, "Transporte", false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "LIC-0002", "María", null, "829-555-0202", "Conductor" },
                    { 3, "Gómez", "001-0000003-3", "cgomez@empresa.com", "Contabilidad", false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Carlos", "Parada Los Guaricanos", null, "Pasajero" },
                    { 4, "Martínez", "001-0000004-4", "amartinez@empresa.com", "Recursos Humanos", false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Ana", "Av. Charles de Gaulle", null, "Pasajero" },
                    { 5, "Fernández", "001-0000005-5", "lfernandez@empresa.com", "Tecnología", false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Luis", "Puente Juan Bosch", null, "Pasajero" },
                    { 6, "Santos", "001-0000006-6", "lsantos@empresa.com", "Ventas", false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Laura", "Av. 27 de Febrero", null, "Pasajero" }
                });

            migrationBuilder.InsertData(
                table: "Rutas",
                columns: new[] { "Id", "Destino", "DuracionEstimadaMinutos", "Eliminado", "FechaCreacion", "FechaEliminacion", "FechaModificacion", "Nombre", "Origen", "ParadasIntermedias" },
                values: new object[,]
                {
                    { 1, "Sede Central, Av. Winston Churchill", 60, false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Ruta Norte - Villa Mella", "Villa Mella, Santo Domingo Norte", "[\"Los Guaricanos\",\"Av. Charles de Gaulle\",\"Puente de la 17\"]" },
                    { 2, "Sede Central, Av. Winston Churchill", 45, false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Ruta Este - Los Mina", "Los Mina, Santo Domingo Este", "[\"Av. San Vicente de Paul\",\"Puente Juan Bosch\"]" },
                    { 3, "Sede Central, Av. Winston Churchill", 40, false, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Ruta Oeste - Herrera", "Herrera, Santo Domingo Oeste", "[\"Av. Isabel Aguiar\",\"Av. 27 de Febrero\"]" }
                });

            migrationBuilder.InsertData(
                table: "Vehiculos",
                columns: new[] { "Id", "Anio", "Capacidad", "Eliminado", "Estado", "FechaCreacion", "FechaEliminacion", "FechaModificacion", "Marca", "Modelo", "Placa" },
                values: new object[,]
                {
                    { 1, 2022, 14, false, "Disponible", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Toyota", "Hiace", "I000101" },
                    { 2, 2021, 25, false, "Disponible", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hyundai", "County", "I000102" },
                    { 3, 2019, 12, false, "EnMantenimiento", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Nissan", "Urvan", "I000103" }
                });

            migrationBuilder.InsertData(
                table: "Viajes",
                columns: new[] { "Id", "ConductorId", "Eliminado", "Estado", "FechaCreacion", "FechaEliminacion", "FechaHoraLlegada", "FechaHoraSalida", "FechaModificacion", "RutaId", "Turno", "VehiculoId" },
                values: new object[,]
                {
                    { 1, 1, false, "Completado", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 11, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 11, 10, 7, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Manana", 1 },
                    { 2, 1, false, "Programado", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 12, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Manana", 1 },
                    { 3, 2, false, "Programado", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2026, 12, 1, 7, 45, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "Manana", 2 }
                });

            migrationBuilder.InsertData(
                table: "ViajePasajeros",
                columns: new[] { "EmpleadoId", "ViajeId", "FechaInscripcion" },
                values: new object[,]
                {
                    { 3, 1, new DateTime(2026, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 1, new DateTime(2026, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 2, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 2, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 3, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Cedula",
                table: "Empleados",
                column: "Cedula",
                unique: true,
                filter: "[Eliminado] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_Nombre",
                table: "Rutas",
                column: "Nombre",
                unique: true,
                filter: "[Eliminado] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Placa",
                table: "Vehiculos",
                column: "Placa",
                unique: true,
                filter: "[Eliminado] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ViajePasajeros_EmpleadoId",
                table: "ViajePasajeros",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_ConductorId_FechaHoraSalida",
                table: "Viajes",
                columns: new[] { "ConductorId", "FechaHoraSalida" });

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_RutaId",
                table: "Viajes",
                column: "RutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_VehiculoId_FechaHoraSalida",
                table: "Viajes",
                columns: new[] { "VehiculoId", "FechaHoraSalida" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViajePasajeros");

            migrationBuilder.DropTable(
                name: "Viajes");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Rutas");

            migrationBuilder.DropTable(
                name: "Vehiculos");
        }
    }
}
