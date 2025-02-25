namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class UserRoleConfigrations : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            //Default Data
            var adminUserRole = new IdentityUserRole<string>
            {
                UserId = DefaultUsers.AdminId,
                RoleId = DefaultRoles.AdminRoleId
            };

            builder.HasData(adminUserRole);
        }
    }
}
