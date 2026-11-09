namespace MyApp.Data.Models;

public class Team
{
    public int Id { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string CaptainName { get; set; } = string.Empty;
    public int MemberCount { get; set; }
    public string ExperienceLevel { get; set; } = string.Empty;

    public List<Playthrough> Playthroughs { get; set; } = new();

    public override string ToString()
    {
        return $"{TeamName} | Капитан: {CaptainName} | Участников: {MemberCount} | Опыт: {ExperienceLevel}";
    }
}