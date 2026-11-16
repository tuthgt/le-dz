using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Data.Models;

namespace MyApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        // Подключаем кнопки к обработчикам.
        Get<Button>("AddTeamButton").Click += AddTeam_Click;
        Get<Button>("DeleteTeamButton").Click += DeleteTeam_Click;
        Get<Button>("SaveTeamButton").Click += SaveTeam_Click;
        Get<ListBox>("TeamsList").SelectionChanged += TeamsList_SelectionChanged;

        Get<Button>("AddQuestButton").Click += AddQuest_Click;
        Get<Button>("DeleteQuestButton").Click += DeleteQuest_Click;

        Get<Button>("AddPlaythroughButton").Click += AddPlaythrough_Click;
        Get<Button>("DeletePlaythroughButton").Click += DeletePlaythrough_Click;

        // Создаём базу и заполняем её начальными данными, если она пустая.
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            DbSeed.Seed(db);
        }

        RefreshAllLists();
    }

    private T Get<T>(string name) where T : Control
    {
        return this.FindControl<T>(name)
            ?? throw new InvalidOperationException($"Не найден элемент интерфейса: {name}");
    }

    // После любого изменения обновляем обычные и выпадающие списки.
    private void RefreshAllLists()
    {
        using var db = new AppDbContext();

        var teams = db.Teams
            .AsNoTracking()
            .OrderBy(t => t.TeamName)
            .ToList();

        var quests = db.Quests
            .AsNoTracking()
            .OrderBy(q => q.Title)
            .ToList();

        var playthroughs = db.Playthroughs
            .Include(p => p.Team)
            .Include(p => p.Quest)
            .AsNoTracking()
            .OrderByDescending(p => p.StartDate)
            .ToList();

        Get<ListBox>("TeamsList").ItemsSource = teams;
        Get<ListBox>("QuestsList").ItemsSource = quests;
        Get<ListBox>("PlaythroughsList").ItemsSource = playthroughs;

        Get<ComboBox>("PlaythroughTeamComboBox").ItemsSource = teams;
        Get<ComboBox>("PlaythroughQuestComboBox").ItemsSource = quests;
    }

    private void ClearTeamFields()
    {
        Get<TextBox>("TeamNameBox").Text = string.Empty;
        Get<TextBox>("CaptainNameBox").Text = string.Empty;
        Get<TextBox>("MemberCountBox").Text = string.Empty;
        Get<TextBox>("ExperienceLevelBox").Text = string.Empty;
    }

    private void ClearQuestFields()
    {
        Get<TextBox>("QuestTitleBox").Text = string.Empty;
        Get<TextBox>("QuestGenreBox").Text = string.Empty;
        Get<TextBox>("QuestDifficultyBox").Text = string.Empty;
        Get<TextBox>("QuestFearLevelBox").Text = string.Empty;
        Get<TextBox>("QuestDurationBox").Text = string.Empty;
    }

    private bool TryReadTeam(
        out string name,
        out string captain,
        out int memberCount,
        out string experience,
        out string error)
    {
        name = Get<TextBox>("TeamNameBox").Text?.Trim() ?? string.Empty;
        captain = Get<TextBox>("CaptainNameBox").Text?.Trim() ?? string.Empty;
        experience = Get<TextBox>("ExperienceLevelBox").Text?.Trim() ?? string.Empty;
        memberCount = 0;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            error = "Введите название команды.";
        else if (string.IsNullOrWhiteSpace(captain))
            error = "Введите имя капитана.";
        else if (!int.TryParse(Get<TextBox>("MemberCountBox").Text, out memberCount))
            error = "Количество участников должно быть целым числом.";
        else if (memberCount <= 0)
            error = "Количество участников должно быть больше нуля.";
        else if (string.IsNullOrWhiteSpace(experience))
            error = "Введите уровень опыта.";

        return error.Length == 0;
    }

    private void TeamsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selected = Get<ListBox>("TeamsList").SelectedItem as Team;

        if (selected is null)
        {
            ClearTeamFields();
            return;
        }

        Get<TextBox>("TeamNameBox").Text = selected.TeamName;
        Get<TextBox>("CaptainNameBox").Text = selected.CaptainName;
        Get<TextBox>("MemberCountBox").Text = selected.MemberCount.ToString();
        Get<TextBox>("ExperienceLevelBox").Text = selected.ExperienceLevel;
    }

    private void AddTeam_Click(object? sender, RoutedEventArgs e)
    {
        if (!TryReadTeam(out var name, out var captain, out var count, out var experience, out var error))
        {
            Get<TextBlock>("TeamMessage").Text = error;
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                db.Teams.Add(new Team
                {
                    TeamName = name,
                    CaptainName = captain,
                    MemberCount = count,
                    ExperienceLevel = experience
                });

                db.SaveChanges();
            }

            RefreshAllLists();
            ClearTeamFields();
            Get<TextBlock>("TeamMessage").Text = "Команда успешно добавлена.";
        }
        catch (Exception ex)
        {
            Get<TextBlock>("TeamMessage").Text = "Ошибка сохранения: " + ex.Message;
        }
    }

    private void DeleteTeam_Click(object? sender, RoutedEventArgs e)
    {
        var selected = Get<ListBox>("TeamsList").SelectedItem as Team;
        if (selected is null)
        {
            Get<TextBlock>("TeamMessage").Text = "Сначала выберите команду.";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var team = db.Teams.FirstOrDefault(t => t.Id == selected.Id);
                if (team is null)
                {
                    Get<TextBlock>("TeamMessage").Text = "Команда не найдена.";
                    return;
                }

                db.Teams.Remove(team);
                db.SaveChanges();
            }

            RefreshAllLists();
            ClearTeamFields();
            Get<TextBlock>("TeamMessage").Text = "Команда удалена.";
        }
        catch (Exception ex)
        {
            Get<TextBlock>("TeamMessage").Text = "Ошибка удаления: " + ex.Message;
        }
    }

    private void SaveTeam_Click(object? sender, RoutedEventArgs e)
    {
        var selected = Get<ListBox>("TeamsList").SelectedItem as Team;
        if (selected is null)
        {
            Get<TextBlock>("TeamMessage").Text = "Сначала выберите команду для изменения.";
            return;
        }

        if (!TryReadTeam(out var name, out var captain, out var count, out var experience, out var error))
        {
            Get<TextBlock>("TeamMessage").Text = error;
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var team = db.Teams.FirstOrDefault(t => t.Id == selected.Id);
                if (team is null)
                {
                    Get<TextBlock>("TeamMessage").Text = "Команда не найдена.";
                    return;
                }

                team.TeamName = name;
                team.CaptainName = captain;
                team.MemberCount = count;
                team.ExperienceLevel = experience;
                db.SaveChanges();
            }

            RefreshAllLists();
            ClearTeamFields();
            Get<TextBlock>("TeamMessage").Text = "Изменения команды сохранены.";
        }
        catch (Exception ex)
        {
            Get<TextBlock>("TeamMessage").Text = "Ошибка сохранения: " + ex.Message;
        }
    }

    private bool TryReadQuest(
        out string title,
        out string genre,
        out int difficulty,
        out int fearLevel,
        out int duration,
        out string error)
    {
        title = Get<TextBox>("QuestTitleBox").Text?.Trim() ?? string.Empty;
        genre = Get<TextBox>("QuestGenreBox").Text?.Trim() ?? string.Empty;
        difficulty = 0;
        fearLevel = 0;
        duration = 0;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(title))
            error = "Введите название квеста.";
        else if (string.IsNullOrWhiteSpace(genre))
            error = "Введите жанр квеста.";
        else if (!int.TryParse(Get<TextBox>("QuestDifficultyBox").Text, out difficulty))
            error = "Сложность должна быть целым числом от 1 до 5.";
        else if (difficulty < 1 || difficulty > 5)
            error = "Сложность должна быть от 1 до 5.";
        else if (!int.TryParse(Get<TextBox>("QuestFearLevelBox").Text, out fearLevel))
            error = "Уровень страха должен быть целым числом от 1 до 5.";
        else if (fearLevel < 1 || fearLevel > 5)
            error = "Уровень страха должен быть от 1 до 5.";
        else if (!int.TryParse(Get<TextBox>("QuestDurationBox").Text, out duration))
            error = "Продолжительность должна быть целым числом.";
        else if (duration <= 0)
            error = "Продолжительность должна быть больше нуля.";

        return error.Length == 0;
    }

    private void AddQuest_Click(object? sender, RoutedEventArgs e)
    {
        if (!TryReadQuest(out var title, out var genre, out var difficulty, out var fear, out var duration, out var error))
        {
            Get<TextBlock>("QuestMessage").Text = error;
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                db.Quests.Add(new Quest
                {
                    Title = title,
                    Genre = genre,
                    Difficulty = difficulty,
                    FearLevel = fear,
                    DurationMinutes = duration
                });

                db.SaveChanges();
            }

            RefreshAllLists();
            ClearQuestFields();
            Get<TextBlock>("QuestMessage").Text = "Квест успешно добавлен.";
        }
        catch (Exception ex)
        {
            Get<TextBlock>("QuestMessage").Text = "Ошибка сохранения: " + ex.Message;
        }
    }

    private void DeleteQuest_Click(object? sender, RoutedEventArgs e)
    {
        var selected = Get<ListBox>("QuestsList").SelectedItem as Quest;
        if (selected is null)
        {
            Get<TextBlock>("QuestMessage").Text = "Сначала выберите квест.";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var quest = db.Quests.FirstOrDefault(q => q.Id == selected.Id);
                if (quest is null)
                {
                    Get<TextBlock>("QuestMessage").Text = "Квест не найден.";
                    return;
                }

                db.Quests.Remove(quest);
                db.SaveChanges();
            }

            RefreshAllLists();
            Get<TextBlock>("QuestMessage").Text = "Квест удалён.";
        }
        catch (Exception ex)
        {
            Get<TextBlock>("QuestMessage").Text = "Ошибка удаления: " + ex.Message;
        }
    }

    private void AddPlaythrough_Click(object? sender, RoutedEventArgs e)
    {
        var team = Get<ComboBox>("PlaythroughTeamComboBox").SelectedItem as Team;
        var quest = Get<ComboBox>("PlaythroughQuestComboBox").SelectedItem as Quest;
        var date = Get<CalendarDatePicker>("PlaythroughDatePicker").SelectedDate;
        var time = Get<TimePicker>("PlaythroughTimePicker").SelectedTime;
        var amountText = Get<TextBox>("PaidAmountBox").Text?.Trim() ?? string.Empty;
        var message = Get<TextBlock>("PlaythroughMessage");

        if (team is null)
        {
            message.Text = "Выберите команду.";
            return;
        }

        if (quest is null)
        {
            message.Text = "Выберите квест.";
            return;
        }

        if (date is null)
        {
            message.Text = "Выберите дату начала.";
            return;
        }

        if (!int.TryParse(amountText, out var amount))
        {
            message.Text = "Оплаченная сумма должна быть целым числом.";
            return;
        }

        if (amount < 0)
        {
            message.Text = "Сумма не может быть отрицательной.";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                db.Playthroughs.Add(new Playthrough
                {
                    TeamId = team.Id,
                    QuestId = quest.Id,
                    StartDate = date.Value.Date.Add(time ?? TimeSpan.Zero),
                    PaidAmount = amount,
                    IsCompleted = Get<CheckBox>("IsCompletedCheckBox").IsChecked ?? false
                });

                db.SaveChanges();
            }

            RefreshAllLists();
            Get<TextBox>("PaidAmountBox").Text = string.Empty;
            Get<CheckBox>("IsCompletedCheckBox").IsChecked = false;
            Get<ComboBox>("PlaythroughTeamComboBox").SelectedItem = null;
            Get<ComboBox>("PlaythroughQuestComboBox").SelectedItem = null;
            Get<CalendarDatePicker>("PlaythroughDatePicker").SelectedDate = null;
            Get<TimePicker>("PlaythroughTimePicker").SelectedTime = null;
            message.Text = "Прохождение успешно добавлено.";
        }
        catch (Exception ex)
        {
            message.Text = "Ошибка сохранения: " + ex.Message;
        }
    }

    private void DeletePlaythrough_Click(object? sender, RoutedEventArgs e)
    {
        var selected = Get<ListBox>("PlaythroughsList").SelectedItem as Playthrough;
        var message = Get<TextBlock>("PlaythroughMessage");

        if (selected is null)
        {
            message.Text = "Сначала выберите прохождение.";
            return;
        }

        try
        {
            using (var db = new AppDbContext())
            {
                var playthrough = db.Playthroughs.FirstOrDefault(p => p.Id == selected.Id);
                if (playthrough is null)
                {
                    message.Text = "Прохождение не найдено.";
                    return;
                }

                db.Playthroughs.Remove(playthrough);
                db.SaveChanges();
            }

            RefreshAllLists();
            message.Text = "Прохождение удалено.";
        }
        catch (Exception ex)
        {
            message.Text = "Ошибка удаления: " + ex.Message;
        }
    }
}
