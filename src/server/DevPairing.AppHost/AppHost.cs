using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var db = postgres.AddDatabase("devpairingdb");

builder.AddProject<Projects.DevPairing_Api>("devpairing-api")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
