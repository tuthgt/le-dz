using MyApp.Data.Models;

namespace MyApp.Data;

public static class DbSeed
{
    public static void Seed(AppDbContext db)
    {
        if (db.Teams.Any() || db.Quests.Any() || db.Playthroughs.Any())
            return;

        db.Teams.AddRange(
            new Team
            {
                TeamName = "Shadow Hunters",
                CaptainName = "Алексей",
                MemberCount = 4,
                ExperienceLevel = "Опытные"
            },
            new Team
            {
                TeamName = "Лисички",
                CaptainName = "Мария",
                MemberCount = 3,
                ExperienceLevel = "Новички"
            },
            new Team
            {
                TeamName = "Ночные совы",
                CaptainName = "Дмитрий",
                MemberCount = 5,
                ExperienceLevel = "Средний"
            }
        );

        db.Quests.AddRange(
            new Quest
            {
                Title = "Проклятие особняка",
                Genre = "Хоррор",
                Difficulty = 4,
                FearLevel = 5,
                DurationMinutes = 60
            },
            new Quest
            {
                Title = "Лаборатория 404",
                Genre = "Научная фантастика",
                Difficulty = 3,
                FearLevel = 2,
                DurationMinutes = 75
            },
            new Quest
            {
                Title = "Тайна пирамиды",
                Genre = "Приключения",
                Difficulty = 2,
                FearLevel = 1,
                DurationMinutes = 60
            }
        );

        db.SaveChanges();

        db.Playthroughs.AddRange(
            new Playthrough
            {
                TeamId = 1,
                QuestId = 1,
                StartDate = DateTime.Today.AddDays(-10).AddHours(18),
                PaidAmount = 4000,
                IsCompleted = true
            },
            new Playthrough
            {
                TeamId = 2,
                QuestId = 2,
                StartDate = DateTime.Today.AddDays(-7).AddHours(16),
                PaidAmount = 3000,
                IsCompleted = true
            },
            new Playthrough
            {
                TeamId = 3,
                QuestId = 3,
                StartDate = DateTime.Today.AddDays(-5).AddHours(19),
                PaidAmount = 4500,
                IsCompleted = false
            },
            new Playthrough
            {
                TeamId = 1,
                QuestId = 2,
                StartDate = DateTime.Today.AddDays(-2).AddHours(17),
                PaidAmount = 4000,
                IsCompleted = true
            }
        );

        db.SaveChanges();
    }
}