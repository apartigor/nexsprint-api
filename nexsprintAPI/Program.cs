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
using (var scope = app.Services.CreateScope())
{
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // aplica as migrations pendentes
    await appDbContext.Database.MigrateAsync();

    // valida se o banco estiver vazio
    if (!appDbContext.Modulos.Any())
    {
        var querysql = @"
        INSERT INTO modulos (Nome, PDF_Url, Descricao, TotalPaginas, Capa_Url) VALUES
        ('Módulo 1: Introdução ao Agile Modeling', 'pdfs/am1.pdf', 'Fundamentos do Agile Modeling para suportar a colaboração ágil', 4, 'capas/am1.jpg'),
        ('Módulo 2: Práticas Essenciais', 'pdfs/am2.pdf', 'As práticas fundamentais para uma modelagem ágil eficiente', 4, 'capas/am2.jpg'),
        ('Módulo 3: Ferramentas e Técnicas', 'pdfs/am3.pdf', 'Ferramentas práticas para facilitar sessões de modelagem.', 4, 'capas/am3.jpg'),
        ('Módulo 4: Colaboração e Comunicação', 'pdfs/am4.pdf', 'Como a modelagem impulsiona a colaboração ágil', 4, 'capas/am4.jpg'),
        ('Módulo 5: Casos Reais', 'pdfs/am5.pdf', 'Exemplos práticos de aplicação do Agile Modeling.', 4, 'capas/am5.jpg'),
        ('Módulo 6: Introdução ao Design Sprint', 'pdfs/ds1.pdf', 'Primeiro contato com Design Sprint e suas aplicações', 4, 'capas/ds1.jpg'),
        ('Módulo 7: Estrutura dos 5 dias', 'pdfs/ds2.pdf', 'Como é estruturado um Design Sprint completo.', 4, 'capas/ds2.jpg'),
        ('Módulo 8: Técnicas e Ferramentas', 'pdfs/ds3.pdf', 'Ferramentas práticas para potencializar o Sprint.', 4, 'capas/ds3.jpg'),
        ('Módulo 9: Facilitando um Sprint', 'pdfs/ds4.pdf', 'Como liderar um Sprint de maneira eficiente.', 4, 'capas/ds4.jpg'),
        ('Módulo 10: Estudos de Caso', 'pdfs/ds5.pdf', 'Aplicações reais e resultados do Design Sprint.', 4, 'capas/ds5.jpg');
         ";

        await appDbContext.Database.ExecuteSqlRawAsync(querysql);
    }

}

app.Run();
