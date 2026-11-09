namespace MyApp.Data.Models;

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public int FearLevel { get; set; }
    public int DurationMinutes { get; set; }

    public List<Playthrough> Playthroughs { get; set; } = new();

    public override string ToString()
    {
        return $"{Title} | Жанр: {Genre} | Сложность: {Difficulty}/5 | Страх: {FearLevel}/5 | {DurationMinutes} мин.";
    }
}