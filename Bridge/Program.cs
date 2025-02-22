var builder = DistributedApplication.CreateBuilder(args);

/*
var elastic = builder.AddElasticsearch("elastic")
    .WithEnvironment("xpack.security.enrollment.enabled", "true")
    .WithDataBindMount("X:/elastic");
var kibana = builder.AddContainer("kibana", "kibana", "8.17.0")
    .WithEnvironment("ELASTICSEARCH_URL", elastic.Resource.Entrypoint)
    .WithHttpEndpoint(1111, 5601)
    .WaitFor(elastic);
    */

var username = builder.AddParameter("username", "user", secret: true);
var password = builder.AddParameter("password", "password", secret: true);
var rabbit = builder.AddRabbitMQ("messaging", username, password, 63685)
    .WithManagementPlugin();
var dbPassword = builder.AddParameter("dbPassword", "DfdFsd33HGrfg2g", secret: true);
var db = builder.AddPostgres("gateway-postgre", username, dbPassword, port: 54322)
    .WithPgAdmin()
    .WithDataBindMount(@"C:\Cubody\NotificationServiceDb")
    .AddDatabase("gatewaydb");

var email = builder.AddProject<Projects.Email>("email-service")
    .WithReference(rabbit)
    .WaitFor(rabbit);
    //.WaitFor(kibana);
var sms = builder.AddProject<Projects.Sms>("sms-service")
    .WithReference(rabbit)
    .WaitFor(rabbit);
var push = builder.AddProject<Projects.Push>("push-service")
    .WithReference(rabbit)
    .WaitFor(rabbit);

builder.AddProject<Projects.Gateway_Api>("gateway-api")
    .WithReference(rabbit)
    .WithReference(db)
    .WaitFor(email)
    .WaitFor(sms)
    .WaitFor(push)
    .WaitFor(db);

builder.AddProject<Projects.Gateway_Worker>("gateway-worker")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db)
    .WaitFor(rabbit);

builder.AddProject<Projects.Gateway_Consumer>("gateway-consumer")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db)
    .WaitFor(rabbit);

builder.Build().Run();
