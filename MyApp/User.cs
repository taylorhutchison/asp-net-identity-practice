using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyApp
{
    public class User : IdentityUser
    {
        public string Locale { get; set; } = "en-US";
		public string OrgId {get; set;}
    }
	public class Organization {
		public string Id {get; set;}
		public string Name {get; set;}
	}
	public class UserDbContext: IdentityDbContext<User> {
		public UserDbContext(DbContextOptions<UserDbContext> options): base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.Entity<User>(user => user.HasIndex(x => x.Locale).IsUnique(false));
			builder.Entity<Organization>(org => {
				org.ToTable("Organizations");
				org.HasKey(x => x.Id);
				org.HasMany<User>().WithOne().HasForeignKey(x => x.OrgId).IsRequired(false);
			});
		}
	}
}
