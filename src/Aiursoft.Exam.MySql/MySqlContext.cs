using Aiursoft.Exam.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Exam.MySql;

public class MySqlContext(DbContextOptions<MySqlContext> options) : TemplateDbContext(options);
