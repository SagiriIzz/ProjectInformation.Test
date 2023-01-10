namespace ProjectInformation;
public class ProjectDB : DbContext
    {
        public ProjectDB(DbContextOptions<ProjectDB> options) : base(options) { }
        public DbSet<Project> Projects => Set<Project>();
    }
    