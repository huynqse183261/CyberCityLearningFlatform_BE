namespace CyberCity.DTOs.Answers
{
    public class AnswerDto
    {
        public string Uid { get; set; }
        public string SubtopicUid { get; set; }
        public string ExpectedOutput { get; set; }
        public string CheckType { get; set; } // exact, contains, regex, custom
        public bool CaseSensitive { get; set; }
        public bool TrimWhitespace { get; set; }
        public string Explanation { get; set; }
        public string Hints { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SubmitAnswerDto
    {
        public string UserOutput { get; set; }
    }

    public class SubmitAnswerResponseDto
    {
        public bool IsCorrect { get; set; }
        public string ExpectedOutput { get; set; }
        public string Explanation { get; set; }
        public string Hints { get; set; }
        public SubtopicProgressDto Progress { get; set; }
    }

    public class SubtopicProgressDto
    {
        public string Uid { get; set; }
        public string StudentUid { get; set; }
        public string SubtopicUid { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string UserOutput { get; set; }
        public bool IsCorrect { get; set; }
        public int AttemptCount { get; set; }
        public DateTime? LastAttemptedAt { get; set; }
    }
}

