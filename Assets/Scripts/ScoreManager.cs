using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }
    public int BestScore { get; private set; }
    
    private bool isCounting = false;
    private float elapsedTime = 0f;
    public float upgradeTime= 0f;

    private int passedObstacleCount = 0;
    private int dangerScore = 0;

    private const string BEST_SCORE_KEY = "BestScore";

    public enum DangerZone { Bad, Normal, Good, Sharp }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadBestScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isCounting) return;

        elapsedTime += Time.deltaTime;

        Score = Mathf.FloorToInt(GetTimeScore()) + (passedObstacleCount * 10) + dangerScore;
    }

    public void StartScoring()
    {
        ResetScore();
        isCounting = true;
    }

    public void StopScoring()
    {
        isCounting = false;
        TryUpdateBestScore();
    }

    public void AddPassedObstacle()
    {
        passedObstacleCount++;
    }

    public void AddDangerScore(DangerZone zone)
    {
        switch (zone)
        {
            case DangerZone.Good:
                dangerScore += 1;
                break;
            case DangerZone.Sharp:
                dangerScore += 3;
                break;
        }
    }

    private float GetTimeScore()
    {
        if (elapsedTime <= upgradeTime)
            return elapsedTime * 1f;
        else if (elapsedTime <= (upgradeTime *2))
            return upgradeTime * 1f + (elapsedTime - upgradeTime) * 3f;
        else if (elapsedTime <= (upgradeTime * 3))
            return upgradeTime * 1f + upgradeTime * 3f + (elapsedTime - (upgradeTime * 2)) * 5f;
        else
            return upgradeTime * 1f + upgradeTime * 3f + upgradeTime * 5f + (elapsedTime - (upgradeTime * 3)) * 6f;
    }

    public void ResetScore()
    {
        Score = 0;
        elapsedTime = 0f;
        passedObstacleCount = 0;
        dangerScore = 0;
    }

    private void TryUpdateBestScore()
    {
        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(BEST_SCORE_KEY, BestScore);
            PlayerPrefs.Save();
        }
    }

    private void LoadBestScore()
    {
        BestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
    }
}
