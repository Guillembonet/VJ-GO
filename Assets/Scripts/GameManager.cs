using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    Board m_board;
    PlayerManager m_player;

    bool m_hasLevelStarted = false;
    bool m_isGamePlaying = false;
    bool m_isGameOver = false;
    bool m_hasLevelFinished = false;

    public bool HasLevelStarted
    {
        get
        {
            return m_hasLevelStarted;
        }

        set
        {
            m_hasLevelStarted = value;
        }
    }
    public bool IsGamePlaying
    {
        get
        {
            return m_isGamePlaying;
        }

        set
        {
            m_isGamePlaying = value;
        }
    }
    public bool IsGameOver
    {
        get
        {
            return m_isGameOver;
        }

        set
        {
            m_isGameOver = value;
        }
    }
    public bool HasLevelFinished
    {
        get
        {
            return m_hasLevelFinished;
        }

        set
        {
            m_hasLevelFinished = value;
        }
    }

    public float delay = 1f;

    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;

    void Awake()
    {
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
        m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (m_player != null && m_board != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.LogWarning("GAMEMANAGER Error: no player or board found!");
        }
    }

    IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {
        Debug.Log("START LEVEL");
        m_player.playerInput.InputEnabled = false;

        //TODO: remove the following line
        PlayLevel();

        while (!m_hasLevelStarted)
        {
            //m_hasLevelStarted = true;
            yield return null; 
        }

        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }

    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("PLAY LEVEL");
        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.playerInput.InputEnabled = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!m_isGameOver)
        {
            //m_isGameOver = true;
            yield return null;
        }
    }

    IEnumerator EndLevelRoutine()
    {
        Debug.Log("END LEVEL");
        m_player.playerInput.InputEnabled = false;

        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        while (!m_hasLevelFinished)
        {
            //m_hasLevelFinished = true;
            yield return null;
        }
        RestartLevel();
    }

    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel()
    {
        m_hasLevelStarted = true;
    }

    void Update(){
        bool key1 = Input.GetKeyDown(KeyCode.Alpha1);
        bool key2 = Input.GetKeyDown(KeyCode.Alpha2);
        bool key3 = Input.GetKeyDown(KeyCode.Alpha3);
        bool esc = Input.GetKeyDown(KeyCode.Escape);

        string currentScene = SceneManager.GetActiveScene().name;

        if(key1 && currentScene != "Level1"){
            SceneManager.LoadScene("Level1");
        }
        else if(key2 && currentScene != "Level2"){
            SceneManager.LoadScene("Level2");
        }
        else if(key3 && currentScene != "Level3"){
            SceneManager.LoadScene("Level3");
        }
        else if(esc && currentScene != "Menu"){
            SceneManager.LoadScene("Menu");
        }
    }
}