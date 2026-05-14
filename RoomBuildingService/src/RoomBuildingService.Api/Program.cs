using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Api.Middlewares;
using RoomBuildingService.Core.Interfaces;
using RoomBuildingService.Infrastructure.BackgroundWorkers;
using RoomBuildingService.Infrastructure.Messaging;
using RoomBuildingService.Infrastructure.Persistence;
using RoomBuildingService.Infrastructure.Persistence.Repositories;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
// ── Database ────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection"),
sql => sql.MigrationsAssembly("RoomBuildingService.Infrastructure")
));
// ── Repositories ────────────────────────────────────────────
builder.Services.AddScoped<IBuildingRepository,  BuildingRepository>();
builder.Services.AddScoped<IRoomTypeRepository,  RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository,      RoomRepository>();
builder.Services.AddScoped<IBedRepository,       BedRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
// ── Messaging + Background Worker ───────────────────────────
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddHostedService<OutboxWorker>();
// ── Exception Handler ────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// ── Controllers + Swagger ────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// ── CORS ────────────────────────────────────────────────────
builder.Services.AddCors(opt =>
opt.AddPolicy("AllowAll", p =>
p.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()));
var app = builder.Build();
// ── Auto migrate khi khởi động ───────────────────────────────
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();
}
// ── Middleware pipeline ──────────────────────────────────────
// THÊM 2 DÒNG NÀY
app.MapOpenApi();
app.MapScalarApiReference(opt =>
{
    opt.Title  = "Room & Building Service API";
    opt.Theme  = ScalarTheme.Purple;
});
app.MapGet("/", () => Results.Redirect("/scalar/v1"));
app.UseExceptionHandler(_ => { });
app.UseCors("AllowAll");
app.MapControllers();
app.Run();