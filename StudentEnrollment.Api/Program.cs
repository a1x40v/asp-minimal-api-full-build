using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Data;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("SchoolDbConnection");
builder.Services.AddDbContext<StudentEnrollmentDbContext>(opt =>
{
    opt.UseNpgsql(conn);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", policy => policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapGet("/courses", async (StudentEnrollmentDbContext dbContext) =>
{
    return await dbContext.Courses.ToListAsync();
});

app.MapGet("/courses/{id}", async (StudentEnrollmentDbContext dbContext, int id) =>
{
    return await dbContext.Courses.FindAsync(id) is Course course ? Results.Ok(course) : Results.NotFound();
});

app.MapPost("/courses", async (StudentEnrollmentDbContext dbContext, Course course) =>
{
    dbContext.Courses.Add(course);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/courses/{course.Id}", course);
});

app.MapPut("/courses/{id}", async (StudentEnrollmentDbContext dbContext, int id, Course course) =>
{
    var record = await dbContext.Courses.FindAsync(id);
    if (record == null) return Results.NotFound();

    record.Credits = course.Credits;
    record.Title = course.Title;

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/courses/{id}", async (StudentEnrollmentDbContext dbContext, int id) =>
{
    var record = await dbContext.Courses.FindAsync(id);
    if (record == null) return Results.NotFound();

    dbContext.Remove(record);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
