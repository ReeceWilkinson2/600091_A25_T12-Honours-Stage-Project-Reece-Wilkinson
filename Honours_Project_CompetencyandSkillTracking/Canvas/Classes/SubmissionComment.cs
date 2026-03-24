namespace Honours_Project_CompetencyandSkillTracking.Canvas.Classes
{
    public class SubmissionComment
    {
        public long Id { get; set; }
        public long SubmissionId { get; set; }
        public Submission Submission { get; set; } = null!;
        public string Comment { get; set; } = "";
        public string? AuthorName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
