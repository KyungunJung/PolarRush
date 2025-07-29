using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Controller player;
    [SerializeField] private MagnetSpawner magnetSpawner;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private ScoreManager scoreManager;

    public void OnGameOver()
    {
        scoreManager.StopScoring();
    }
    public void OnStartButtonPressed()
    {
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
    }
    public void StartBtn()
    {
        FindObjectOfType<Controller>().ResetController();

        FindObjectOfType<MagnetSpawner>().ResetAllMagnetPairs();
    }
}
