using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gamePause;
    [SerializeField] GameObject gameIntro;
    [SerializeField] GameObject ScoorePanel;
    [SerializeField] TextMeshProUGUI scooreText;
    [SerializeField] TextMeshProUGUI gameOverScooreText;
    [SerializeField] PlayerController player;
    [SerializeField] Image[] hearts;
    public Transform RespownPoint;
    public int Lifes = 3;
    int score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = 0;
        ShowIntroPanel(true);
        ShowScoorePanel(false);
    }

    private void Start()
    {
        scooreText.text = "Score: " + score;
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (gameIntro.activeSelf)
            {
                Play();
            }
            else if (gamePause.activeSelf)
            {
                Resume();
            }
        }
    }

    public void ShowGameOverPanel(bool state)
    {
        gameOverPanel.SetActive(state);
        gameOverScooreText.text = "Final Score: " + score;
    }

    public void ShowPausePanel(bool state)
    {
        gamePause.SetActive(state);
    }
    public void ShowIntroPanel(bool state)
    {
        gameIntro.SetActive(state);
    }

    public void ShowScoorePanel(bool state)
    {
        ScoorePanel.SetActive(state);
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scooreText.text = "Score: " + score;
    }

    public void UpdateLifes()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < Lifes;
        }

        if (Lifes <= 0)
        {
            Lifes = 0;
            Time.timeScale = 0;
            ShowGameOverPanel(true);
        }
    }

    public void Reset()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Play()
    {
        ShowIntroPanel(false);
        ShowScoorePanel(true);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        ShowPausePanel(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        ShowPausePanel(false);
    }

    public void Respawn()
    {
        player.transform.position = RespownPoint.position;

        player.CurrentHealth = player.MaxHealth;

        player.ResetState();
    }
}
