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
                Id = DefaultUsers.AdminId,
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                FirstName = DefaultUsers.FirstName,
                LastName = DefaultUsers.LastName,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                EmailConfirmed = true,
                PasswordHash = DefaultUsers.AdminPasswordHash
            };

            builder.HasData(adminUser);
        }
    }
}
