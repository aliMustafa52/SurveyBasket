namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class ApplicationUserConfigrations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .OwnsMany(x => x.RefreshTokens)
                .ToTable("RefreshTokens")
                .WithOwner()
                .HasForeignKey("UserId");

            builder.Property(x => x.FirstName)
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .HasMaxLength(100);

            //Default Data
            var adminUser = new ApplicationUser
            {
                Id = DefaultUsers.Admin.Id,
                Email = DefaultUsers.Admin.Email,
                NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
                UserName = DefaultUsers.Admin.Email,
                NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
                FirstName = DefaultUsers.Admin.FirstName,
                LastName = DefaultUsers.Admin.LastName,
                ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
                SecurityStamp = DefaultUsers.Admin.SecurityStamp,
                EmailConfirmed = true,
                PasswordHash = DefaultUsers.Admin.PasswordHash
            };

            builder.HasData(adminUser);
        }
    }
}
