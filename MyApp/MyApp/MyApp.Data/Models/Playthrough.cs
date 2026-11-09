namespace MyApp.Data.Models;

public class Playthrough
{
    public int Id { get; set; }

    public int TeamId { get; set; }
    public int QuestId { get; set; }

    public DateTime StartDate { get; set; }
    public int PaidAmount { get; set; }
    public bool IsCompleted { get; set; }

    public Team Team { get; set; } = null!;
    public Quest Quest { get; set; } = null!;

    public override string ToString()
    {
        string teamName = Team?.TeamName ?? $"Команда #{TeamId}";
        string questName = Quest?.Title ?? $"Квест #{QuestId}";

        return $"{StartDate:g} | {teamName} | {questName} | {PaidAmount} руб. | {(IsCompleted ? "Завершён" : "Не завершён")}";
    }
}