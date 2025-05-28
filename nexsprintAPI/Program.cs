using Microsoft.EntityFrameworkCore;
using nexsprintAPI.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(); 
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

// query SQL inicial para adicionar os pdfs
// using (var scope = app.Services.CreateScope())
// {
//     var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//     // aplica as migrations pendentes
//     await appDbContext.Database.MigrateAsync();

//     // valida se o banco estiver vazio
//     if (!appDbContext.Modulos.Any())
//     {
//         var querysql = @"
//         
//         ";

//         await appDbContext.Database.ExecuteSqlRawAsync(querysql);
//     }

// }

app.Run();
