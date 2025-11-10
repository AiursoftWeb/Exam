using Aiursoft.DbTools;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Exam.Entities;

public abstract class TemplateDbContext(DbContextOptions options) : IdentityDbContext<User>(options), ICanMigrate
{
    public virtual  Task MigrateAsync(CancellationToken cancellationToken) =>
        Database.MigrateAsync(cancellationToken);

    public virtual  Task<bool> CanConnectAsync() =>
        Database.CanConnectAsync();

    public DbSet<ExamPaperSubmission> ExamPaperSubmissions => Set<ExamPaperSubmission>();

    public DbSet<ExamPaper> ExamPapers => Set<ExamPaper>();

    public DbSet<ExamPaperQuestionAnswer> ExamPaperQuestionAnswers => Set<ExamPaperQuestionAnswer>();

    public DbSet<ExamPaperQuestion> ExamPaperQuestions => Set<ExamPaperQuestion>();

    public DbSet<Question> Questions => Set<Question>();
    public DbSet<FillInBlankQuestion> FillInBlankQuestion => Set<FillInBlankQuestion>();
    public DbSet<ChoiceQuestion> ChoiceQuestions => Set<ChoiceQuestion>();

    public DbSet<Choice> Choices => Set<Choice>();

}
