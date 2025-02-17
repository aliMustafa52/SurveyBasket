using SurveyBasketV5.Contracts.Questions;

namespace SurveyBasketV5.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //config.NewConfig<QuestionRequest, Question>()
            //    .Ignore(x => x.Answers);

            config.NewConfig<QuestionRequest, Question>()
                .Map(dest => dest.Answers, 
                        src => src.Answers.Select(answer => new Answer { Content = answer }));


        }
    }
}
