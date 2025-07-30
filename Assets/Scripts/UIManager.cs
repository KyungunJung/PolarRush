using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject MainUI;
    public GameObject Pause;
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

    public void GameOver()
    {
        Pause.SetActive(false);
        MainUI.SetActive(true);
        FindObjectOfType<ScoreUI>().OnGameOver();
    }
    public void PauseControll()
    {
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
    }
    public void DeactivePause()
    {
        Pause.GetComponent<Button>().interactable = false;

    }
    public void ActivePause()
    {
        Pause.GetComponent<Button>().interactable = true;
    }
}
