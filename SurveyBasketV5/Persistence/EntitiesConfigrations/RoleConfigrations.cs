namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class RoleConfigrations : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            //Default Data
            var adminRole = new ApplicationRole
            {
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.AdminRoleName,
                NormalizedName = DefaultRoles.AdminRoleName.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
            };

            var MemberRole = new ApplicationRole
            {
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.MemberRoleName,
                NormalizedName = DefaultRoles.MemberRoleName.ToUpper(),
                ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                IsDefault = true,
            };

            builder.HasData([
                adminRole,
                MemberRole
            ]);
        }
    }
}
