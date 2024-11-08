using gRPC_ModManager.Server.Services;
using gRPC_ModManager.Shared;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(opt => { opt.EnableDetailedErrors = true; });
builder.Services.AddCodeFirstGrpc();
builder.Services.AddSingleton<IDirectorySyncService, DirectorySyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DirectorySyncService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();