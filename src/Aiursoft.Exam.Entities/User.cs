using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Aiursoft.Exam.Entities;

public class User : IdentityUser
{
    public const string DefaultAvatarPath = "Workspace/avatar/default-avatar.jpg";

    [MaxLength(30)]
    [MinLength(2)]
    public required string DisplayName { get; set; }

    [MaxLength(150)]
    [MinLength(2)]
    public string AvatarRelativePath { get; set; } = DefaultAvatarPath;

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    [InverseProperty(nameof(ExamPaperSubmission.User))]
    public IEnumerable<ExamPaperSubmission> ExamPaperSubmissions { get; init; } = new List<ExamPaperSubmission>();
}

public class ExamPaperSubmission
{
    [Key]
    public Guid Id { get; init; }

    public required Guid ExamPaperId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(ExamPaperId))]
    public ExamPaper? ExamPaper { get; init; }

    [MaxLength(256)]
    public required string UserId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public User? User { get; init; }

    /// <summary>
    /// The UTC time when the exam was started.
    /// </summary>
    public DateTime StartTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC time when the exam was submitted. Null if not submitted yet. Null means the exam is still in progress.
    /// </summary>
    public DateTime? SubmissionTime { get; init; }

    public int TotalScore { get; set; }

    [InverseProperty(nameof(ExamPaperQuestionAnswer.ExamPaperSubmission))]
    public IEnumerable<ExamPaperQuestionAnswer> Answers { get; init; } = new List<ExamPaperQuestionAnswer>();
}

public class ExamPaper
{
    [Key]
    public Guid Id { get; init; }

    [StringLength(256)]
    public required string Title { get; set; }

    [StringLength(4096)]
    public string? Description { get; set; }

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    [InverseProperty(nameof(ExamPaperQuestion.ExamPaper))]
    public IEnumerable<ExamPaperQuestion> Questions { get; init; } = new List<ExamPaperQuestion>();

    [InverseProperty(nameof(ExamPaperSubmission.ExamPaper))]
    public IEnumerable<ExamPaperSubmission> Submissions { get; init; } = new List<ExamPaperSubmission>();

    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(60);

    public bool ShuffleQuestions { get; set; }

    public bool AllowRetake { get; set; }

    public int MaxRetakeCount { get; set; } = 5;

    public int PassingScore { get; set; } = 80;
}

public class ExamPaperQuestionAnswer
{
    [Key]
    public Guid Id { get; init; }

    public required Guid ExamPaperQuestionId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(ExamPaperQuestionId))]
    public ExamPaperQuestion? ExamPaperQuestion { get; init; }

    public required Guid ExamPaperSubmissionId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(ExamPaperSubmissionId))]
    public ExamPaperSubmission? ExamPaperSubmission { get; init; }

    public bool IsCorrect { get; set; }

    /// <summary>
    /// The content of the answer provided by the user.
    ///
    /// This is a JSON field that can store different types of answers based on the question type.
    /// For ChoiceQuestion, it can store the selected choice IDs. For example: {"type":"ChoiceAnswer","selectedChoiceIds":["choiceId1","choiceId2"]}
    /// For FillInBlankQuestion, it can store the filled text. For example: {"type":"FillInBlankAnswer","filledText":"The capital of France is Paris."}
    /// </summary>
    [StringLength(8192)]
    public required string AnswerContent { get; set; }

    public int ObtainedScore { get; set; }
}

public class ExamPaperQuestion
{
    [Key]
    public Guid Id { get; init; }

    public required Guid ExamPaperId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(ExamPaperId))]
    public ExamPaper? ExamPaper { get; init; }

    public required Guid QuestionId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(QuestionId))]
    public Question? Question { get; init; }

    public int Order { get; set; }

    public int Score { get; set; } = 5;

    [InverseProperty(nameof(ExamPaperQuestionAnswer.ExamPaperQuestion))]
    public IEnumerable<ExamPaperQuestionAnswer> Answers { get; init; } = new List<ExamPaperQuestionAnswer>();
}

public abstract class Question
{
    [Key]
    public Guid Id { get; init; }

    [StringLength(1024)]
    public required string Discriminator { get; init; }

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    [InverseProperty(nameof(ExamPaperQuestion.Question))]
    public IEnumerable<ExamPaperQuestion> AppearedInExamPapers { get; init; } = new List<ExamPaperQuestion>();
}

public class ChoiceQuestion : Question
{
    [StringLength(4096)]
    public required string Content { get; set; }

    [InverseProperty(nameof(Choice.Question))]
    public IEnumerable<Choice> Choices { get; init; } = new List<Choice>();
}

public class Choice
{
    [Key]
    public Guid Id { get; init; }

    [StringLength(2048)]
    public required string Content { get; set; }

    public bool IsCorrect { get; set; }

    public required Guid? QuestionId { get; set; }

    [NotNull]
    [JsonIgnore]
    [ForeignKey(nameof(QuestionId))]
    public ChoiceQuestion? Question { get; init; }
}

public class FillInBlankQuestion : Question
{
    /// <summary>
    /// We assume that the Content should be: "The capital of France is _____." Where the '_____' is the blank to fill in.
    /// </summary>
    [StringLength(8192)]
    public required string TextWithBlanks { get; set; }

    [StringLength(1024)]
    public required string Answer { get; set; }
}
