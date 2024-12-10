using backend.Hubs;
using backend.Repositories;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IGameRepository, GameRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddSignalR();

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); // Ensure this is before UseEndpoints


app.MapHub<GameHub>("/gameHub");

app.Run();