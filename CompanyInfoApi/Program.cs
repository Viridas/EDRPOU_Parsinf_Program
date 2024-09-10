var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ExcelProcessingService>();
builder.Services.AddScoped<GrainTradeService>();
builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));
app.MapControllers();

app.Run();
