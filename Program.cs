using UploadBobsWorkers;
using UploadBobsWorkers.Interfaces;
using UploadBobsWorkers.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

var host = builder.Build();
host.Run();
