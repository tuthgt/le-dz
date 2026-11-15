using System.Linq;
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;

namespace MyApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        // Создание базы и заполнение начальными данными
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            DbSeed.Seed(db);
        }

        // Загрузка команд
        using (var db = new AppDbContext())
        {
            var teams = db.Teams
                .AsNoTracking()
                .ToList();

            this.FindControl<ListBox>("TeamsList")!.ItemsSource = teams;
        }

        // Загрузка квестов
        using (var db = new AppDbContext())
        {
            var quests = db.Quests
                .AsNoTracking()
                .ToList();

            this.FindControl<ListBox>("QuestsList")!.ItemsSource = quests;
        }

        // Загрузка прохождений с названиями команд и квестов
        using (var db = new AppDbContext())
        {
            var playthroughs = db.Playthroughs
                .Include(p => p.Team)
                .Include(p => p.Quest)
                .AsNoTracking()
                .OrderByDescending(p => p.StartDate)
                .ToList();

            this.FindControl<ListBox>("PlaythroughsList")!.ItemsSource = playthroughs;
        }
    }
}