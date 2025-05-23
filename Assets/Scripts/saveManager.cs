using UnityEngine;

public class saveManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";
    // 保存最高分数
    public static void SaveHighScore(int score)
    {
        if(score > LoadHighScore())
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }
    }

    // 加载最高分数
    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }
}
