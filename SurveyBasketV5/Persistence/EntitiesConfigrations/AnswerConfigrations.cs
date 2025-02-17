namespace SurveyBasketV5.Persistence.EntitiesConfigrations
{
    public class AnswerConfigrations : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasIndex(x => new {x.Content, x.QuestionId})
                .HasFilter("IsActive = 1")
                .IsUnique();

            builder.Property(x => x.Content)
                .HasMaxLength(1000);
        }
    }
}
