using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Api.Middlewares;
using RoomBuildingService.Core.Interfaces;
using RoomBuildingService.Infrastructure.BackgroundWorkers;
using RoomBuildingService.Infrastructure.Messaging;
using RoomBuildingService.Infrastructure.Persistence;
using RoomBuildingService.Infrastructure.Persistence.Repositories;
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
builder.Services.AddSwaggerGen();
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
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Room & Building Service API v1");
    c.RoutePrefix = "swagger";
});
app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseExceptionHandler(_ => { });
app.UseCors("AllowAll");
app.MapControllers();
app.Run();