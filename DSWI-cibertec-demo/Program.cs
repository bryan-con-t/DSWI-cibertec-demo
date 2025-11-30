using DSWI_cibertec_demo.Data;
using DSWI_cibertec_demo.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Configurar gRPC
builder.Services.AddGrpc();

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

// Registrar el repositorio ADO.NET
builder.Services.AddScoped<ProductoRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DSWI Cibertec Demo API",
        Version = "v1"
    });
});
builder.Services.AddControllers();

var app = builder.Build();

app.MapGrpcService<ProductoGrpcService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DSWI Cibertec Demo API V1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();