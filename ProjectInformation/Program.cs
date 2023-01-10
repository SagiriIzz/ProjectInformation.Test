using ProjectInformation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProjectDB>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ProjectDB>();
    db.Database.EnsureCreated();
}

app.MapGet("/projects", async (ProjectDB db) => await db.Projects.ToListAsync());
app.MapGet("/project{id}", async (int id, ProjectDB db) => await  db.Projects.FirstOrDefaultAsync(p => p.Id == id)is Project project
? Results.Ok(project)
: Results.NotFound());
app.MapPost("/projects", async ([FromBody] Project project,ProjectDB db) =>
{
    db.Projects.Add(project);
    await db.SaveChangesAsync();
    return Results.Created($"projects/{project.Id}", project);
});
app.MapPut("/projects",async ([FromBody] Project project,ProjectDB db) =>
{
    var projectFromDb = await db.Projects.FindAsync(new object[] {project.Id});
    if (projectFromDb == null) return Results.NotFound();
    projectFromDb.Name = project.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/projects/{id}", async (int id, ProjectDB db) =>
{
    var ProjectFromDb = await db.Projects.FindAsync(new object[] {id});
    if (ProjectFromDb == null) return Results.NotFound();
    db.Projects.Remove(ProjectFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.UseHttpsRedirection();

app.Run();


