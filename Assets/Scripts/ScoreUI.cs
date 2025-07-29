using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ScoreManager scoreManager;

    void Update()
    {
        scoreText.text = $"Score: {scoreManager.Score}";
    }
}
