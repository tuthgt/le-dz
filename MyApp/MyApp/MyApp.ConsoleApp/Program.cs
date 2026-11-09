using System.Text;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Questpoint");

Console.WriteLine("\nБаза данных:");
Console.WriteLine(AppDbContext.DbPath);

using var db = new AppDbContext();

db.Database.EnsureCreated();

DbSeed.Seed(db);

Console.WriteLine("\nКоманды");

foreach (var team in db.Teams.OrderBy(t => t.Id))
{
    Console.WriteLine(team);
}

Console.WriteLine("\nКвесты");

foreach (var quest in db.Quests.OrderBy(q => q.Id))
{
    Console.WriteLine(quest);
}

Console.WriteLine("\nПрохождения");

var playthroughs = db.Playthroughs
    .Include(p => p.Team)
    .Include(p => p.Quest)
    .OrderBy(p => p.Id);

foreach (var playthrough in playthroughs)
{
    Console.WriteLine(playthrough);
}

Console.WriteLine("\nРабота программы завершена.");