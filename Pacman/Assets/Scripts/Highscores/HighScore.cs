
/// <summary>
/// Class that represents a HighScore on disk (name, score)
/// </summary>
[System.Serializable]
public class HighScore
{
    public string name;
    public int score;

    /// <summary>
    /// Constructs a new highscore with a name and score
    /// </summary>
    /// <param name="name">The name of who got the highscore</param>
    /// <param name="score">The value of the score</param>
    public HighScore(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
