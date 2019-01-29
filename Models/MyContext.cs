    using Microsoft.EntityFrameworkCore; 
    namespace OpenMicChicago.Models
    {
        public class MyContext : DbContext
        {
            public MyContext(DbContextOptions<MyContext> options) : base(options) { }
            
            // "users" table is represented by this DbSet "Users"
            public DbSet<User> Users {get;set;}
            public DbSet<OpenMic> OpenMics {get;set;}
            public DbSet<Like> Likes {get;set;}
            
            public DbSet<Venue> Venues {get;set;}
            
        }
    }
    