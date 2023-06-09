using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FollowPlayer : MonoBehaviour
{
    private GameObject ball;
    private AudioSource audioPlayer;
    public int score = 0;
    public TextMeshProUGUI scoreText, timeText, gameOverText, pauseGameText;
    public float time = 60.0f;
    public Button restartButton,pauseButton;
    public bool gameOver = false, pauseFlag = false;
    public FixedJoystick rotateJoystick;
    private float rotationValue = 250.0f;
    private BallMovement _ballMovementScript;

    private int timeVar = 1;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.Find("Ball");
        audioPlayer = GetComponent<AudioSource>();
        _ballMovementScript = GameObject.Find("Ball").GetComponent<BallMovement>();
     
        pauseButton.onClick.AddListener(PauseGame);   
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = ball.transform.position;
        // transform.Rotate(new Vector3(0,Input.GetAxis("Mouse X"),0) * Time.deltaTime * rotationValue);
        rotationValue = 65.0f;
        transform.Rotate(new Vector3(0,rotateJoystick.Horizontal,0) * Time.deltaTime * rotationValue);
        
        scoreText.text = "Score : " + score;

        if(gameOver)
            GameOver();
        
        if (time > 0)
        {
            time -= (Time.deltaTime * timeVar);
            timeText.text = "Time : " + Math.Round(time);
        }
        else
            gameOver = true;
    }

    void GameOver()
    {
        gameOver = true;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        pauseGameText.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        audioPlayer.Pause();
        timeVar = 0;
    }

    void PauseGame()
    {
        pauseFlag = !pauseFlag;

        if (pauseFlag)
        {
            timeVar = 0;
            pauseGameText.gameObject.SetActive(true);
            audioPlayer.Pause();
        }
        else
        {
            timeVar = 1;
            pauseGameText.gameObject.SetActive(false);
            audioPlayer.Play(0);
        }

    }
    public void RestartGame()
    {
        transform.position = _ballMovementScript.startPos;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
