namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class QuestionConfigrations : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasIndex(x => new { x.Content, x.PollId })
                .HasFilter("IsActive = 1")
                .IsUnique();

            builder.Property(x => x.Content)
                .HasMaxLength(1000);
        }
    }
}
