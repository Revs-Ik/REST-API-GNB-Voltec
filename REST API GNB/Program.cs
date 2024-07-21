using REST_API_GNB.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Exception Handler
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();
