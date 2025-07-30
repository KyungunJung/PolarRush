using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestText;
    [SerializeField] private ScoreManager scoreManager;

    void Update()
    {
        scoreText.text = $"Score: {scoreManager.Score}";
    }
    public void OnGameOver()
    {
        ScoreManager.Instance.StopScoring();

        int bestScore = ScoreManager.Instance.BestScore;

        // UI¿¡ Ãâ·Â
        bestText.text = $"Best: {bestScore}";
    }

}
