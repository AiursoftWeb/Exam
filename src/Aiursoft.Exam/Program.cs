using Aiursoft.DbTools;
using Aiursoft.Exam.Entities;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Exam;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var app = await AppAsync<Startup>(args);
        await app.UpdateDbAsync<TemplateDbContext>();
        await app.SeedAsync();
        await app.CopyAvatarFileAsync();
        await app.RunAsync();
    }
}
