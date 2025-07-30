using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("���� ������Ʈ ����")]
    [SerializeField] private Controller player;
    [SerializeField] private MagnetSpawner magnetSpawner;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private ScoreManager scoreManager;

    [Header("���� �̵� �ӵ� ����")]
    [SerializeField] private float globalMoveSpeed = 2f;
    [SerializeField] private float speedIncreaseInterval = 10f;
    [SerializeField] private float speedIncrement = 0.5f;

    private float elapsedTime = 0f;

    public float GlobalMoveSpeed => globalMoveSpeed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // ���� �̵� �ӵ� ����
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= speedIncreaseInterval)
        {
            globalMoveSpeed += speedIncrement;
            elapsedTime = 0f;
            Debug.Log($"[�ӵ� ����] ���� ���� �ӵ�: {globalMoveSpeed}");
        }
    }

    public void OnGameOver()
    {
        scoreManager.StopScoring();
    }

    public void OnStartButtonPressed()
    {
        scoreManager.ResetScore();
        UIManager.Instance.DeactivePause();
        Time.timeScale = 0f;

        player.ResetController();
        magnetSpawner.ResetAllMagnetPairs();

        player.IsControllable = false;

        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        int count = 3;
        countdownText.gameObject.SetActive(true);

        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f);
            count--;
        }

        countdownText.text = "START!";
        yield return new WaitForSecondsRealtime(0.7f);

        countdownText.gameObject.SetActive(false);
        player.IsControllable = true;

        Time.timeScale = 1f;
        scoreManager.StartScoring();

        player.SetRadius();
        UIManager.Instance.ActivePause();
    }
}
