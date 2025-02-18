namespace SurveyBasketV5.Errors
{
    public static class VoteErrors
    {
        public static readonly Error InvalidQuestions =
            new("Poll.InvalidQuestions", "InvalidQuestions", StatusCodes.Status404NotFound);

        public static readonly Error DuplicatedVote =
            new("Vote.DuplicatedVote", "You have voted to this poll before", StatusCodes.Status409Conflict);
    }
}
