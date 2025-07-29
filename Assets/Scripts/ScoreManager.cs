using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }

    private bool isCounting = false;
    private float elapsedTime = 0f;

    private int passedObstacleCount = 0;
    private int dangerScore = 0;

    public enum DangerZone { Bad, Normal, Good, Sharp }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isCounting) return;

        elapsedTime += Time.deltaTime;

        Score = Mathf.FloorToInt(GetTimeScore()) + (passedObstacleCount * 10) + dangerScore;

        // Debug.Log($"TimeScore: {GetTimeScore()}, Obstacles: {passedObstacleCount}, Danger: {dangerScore}, Total: {Score}");
    }

    public void StartScoring()
    {
        ResetScore();
        isCounting = true;
    }

    public void StopScoring()
    {
        isCounting = false;
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
                // Bad, Normal 은 점수 없음
        }
    }

    private float GetTimeScore()
    {
        if (elapsedTime <= 60f)
            return elapsedTime * 1f;
        else if (elapsedTime <= 120f)
            return 60f * 1f + (elapsedTime - 60f) * 3f;
        else if (elapsedTime <= 180f)
            return 60f * 1f + 60f * 3f + (elapsedTime - 120f) * 5f;
        else
            return 60f * 1f + 60f * 3f + 60f * 5f + (elapsedTime - 180f) * 6f;
    }

    public void ResetScore()
    {
        Score = 0;
        elapsedTime = 0f;
        passedObstacleCount = 0;
        dangerScore = 0;
    }
}
