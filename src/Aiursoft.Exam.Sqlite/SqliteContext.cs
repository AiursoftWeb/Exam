using Aiursoft.Exam.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Exam.Sqlite;

public class SqliteContext(DbContextOptions<SqliteContext> options) : TemplateDbContext(options)
{
    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
