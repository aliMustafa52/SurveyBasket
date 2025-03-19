namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class UserRoleConfigrations : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            //Default Data
            var adminUserRole = new IdentityUserRole<string>
            {
                UserId = DefaultUsers.Admin.Id,
                RoleId = DefaultRoles.Admin.Id
            };

            builder.HasData(adminUserRole);
        }
    }
}
