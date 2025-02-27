var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;

// Add services to the container.
builder.Services.AddCarter(new DependencyContextAssemblyCatalog([assembly]));
builder.Services.AddMediatR(configuration => { configuration.RegisterServicesFromAssembly(assembly); });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();

app.Run();