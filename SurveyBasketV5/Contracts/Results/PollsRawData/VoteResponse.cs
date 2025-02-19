namespace SurveyBasketV5.Contracts.Results.PollsRawData
{
    public record VoteResponse
    (
        string VoterName,
        DateTime VoteDate,
        IEnumerable<QuestionAnswerResponse> SelectedAnswer
    );
}
