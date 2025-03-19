namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class RoleConfigrations : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            //Default Data
            var adminRole = new ApplicationRole
            {
                Id = DefaultRoles.Admin.Id,
                Name = DefaultRoles.Admin.Name,
                NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp,
            };

            var MemberRole = new ApplicationRole
            {
                Id = DefaultRoles.Member.Id,
                Name = DefaultRoles.Member.Name,
                NormalizedName = DefaultRoles.Member.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Member.ConcurrencyStamp,
                IsDefault = true,
            };

            builder.HasData([
                adminRole,
                MemberRole
            ]);
        }
    }
}
