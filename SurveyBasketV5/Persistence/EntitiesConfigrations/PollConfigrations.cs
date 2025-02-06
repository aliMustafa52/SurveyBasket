namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class PollConfigrations : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.HasIndex(x => x.Title)
                .HasFilter("IsActive = 1")
                .IsUnique();

            builder.Property(x => x.Title)
                .HasMaxLength(100);

            builder.Property(x => x.Summary)
                .HasMaxLength(1500);
        }
    }
}
