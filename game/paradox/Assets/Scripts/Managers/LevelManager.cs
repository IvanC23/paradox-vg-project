using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int _currentLevel;
    private Statistics stats;
    private int[] starsPerLevel;
    private float[] completionTimePerLevel;
    private int[] paradoxPerLevel;
    private int[] retryPerLevel;
    private float[] overallTimePerLevel;

    private int _levelsFinished = 0;

    private bool firstTimePlaying = true;
    private bool fromGallery = false;
    private int numOfLevels;
    //private Scene _nextScene;


    //[SerializeField] private GameObject _loaderCanvas;
    //[SerializeField] private Image _progressBar;
    //private float _target;


    private void Awake()
    {
        //IF WE WANT TO ESTABLIH A FRAMERATE WE COULD PUT IT HERE
        //Application.targetFrameRate = 60;

        if (Instance == null)
        {
            Instance = this;
            numOfLevels = SceneManager.sceneCountInBuildSettings - 4;
            LoadData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _currentLevel = SceneManager.GetActiveScene().buildIndex;


    }


    private void LoadScene(int level)
    {
        GameObject.Find("Door").GetComponent<Animator>().SetTrigger("Close");
        StartCoroutine("EndLevel", level);
    }


    public void PlayLevel(int level)
    {
        if (level >= SceneManager.sceneCountInBuildSettings)
        {
            return;
        }

        _currentLevel = level;
        LoadScene(_currentLevel);
    }
    public void PlayMainMenu()
    {
        LoadScene(0);
        _currentLevel = 0;
    }
    public void PlayFirstLevel()
    {
        if (firstTimePlaying)
        {
            //OPEN STORYBOARD
            PlayLevel(32);
        }
        else
        {
            if (_levelsFinished == 31)
            {
                PlayLevel(_levelsFinished);
                return;
            }
            PlayLevel(_levelsFinished + 1);
        }
    }
    public void PlayNextLevel()
    {
        if (_currentLevel + 1 == 32)
        {
            PlayLevel(33);
            return;
        }

        PlayLevel(_currentLevel + 1);
    }
    public void RestartLevel()
    {
        PlayLevel(_currentLevel);
    }
    public bool IsTutorialLevel()
    {
        if (_currentLevel == 1)
            return true;
        return false;
    }

    private IEnumerator EndLevel(int level)
    {
        yield return new WaitForSecondsRealtime(1f);
        var scene = SceneManager.LoadSceneAsync(level);

    }

    public int getLevelsFinished()
    {
        return _levelsFinished;
    }
    public int[] GetStarsPerLevel()
    {
        return starsPerLevel;
    }
    public float[] GetCompletionTimePerLevel()
    {
        return completionTimePerLevel;
    }

    public float[] GetOverallTimePerLevel()
    {
        return overallTimePerLevel;
    }
    public int[] GetParadoxPerLevel()
    {
        return paradoxPerLevel;
    }
    public int[] GetRetryPerLevel()
    {
        return retryPerLevel;
    }

    public void SaveData()
    {
        SaveSystem.SaveData(this);
    }


    public void LoadData()
    {
        GameData Data = SaveSystem.LoadData();
        if (Data != null)
        {
            this._levelsFinished = Data.lastLevelFinished;
            this.starsPerLevel = Data.starsPerLevel;
            this.completionTimePerLevel = Data.completionTimePerLevel;
            this.overallTimePerLevel = Data.overallTimePerLevel;
            this.paradoxPerLevel = Data.paradoxPerLevel;
            this.retryPerLevel = Data.retryPerLevel;
            this._levelsFinished = Data.lastLevelFinished;
            this.firstTimePlaying = false;
            //ScanDebug();
        }
        else
        {
            starsPerLevel = new int[numOfLevels];
            completionTimePerLevel = new float[numOfLevels];
            overallTimePerLevel = new float[numOfLevels];
            paradoxPerLevel = new int[numOfLevels];
            retryPerLevel = new int[numOfLevels];
            _levelsFinished = -1;
            for (int i = 0; i < numOfLevels; i++)
            {
                starsPerLevel[i] = 0;
                completionTimePerLevel[i] = 0f;
                overallTimePerLevel[i] = 0f;
                retryPerLevel[i] = 0;
                paradoxPerLevel[i] = 0;
            }
        }
    }

    public Statistics GetStatisticsLevel()
    {
        return stats;
    }

    public void ScanDebug()
    {

        for (int i = 0; i < numOfLevels; i++)
        {
            Debug.Log("Completion time for level " + i + " is :" + completionTimePerLevel[i]);
            Debug.Log("Num of stars earned in level " + i + " is :" + starsPerLevel[i]);
            Debug.Log("Num of paradox in level " + i + " is :" + paradoxPerLevel[i]);
            Debug.Log("Num of retrial in level " + i + " is :" + retryPerLevel[i]);
            Debug.Log("Overall time for level " + i + " is :" + overallTimePerLevel[i]);
        }
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public void GetStats(Statistics gamestats)
    {
        //Debug.Log("Entered here");

        stats = gamestats;

        /*if (stats != null)
        {
            Debug.Log("Found stats");
            Debug.Log(stats.GetStars());
            Debug.Log(stats.GetCompletionTime());
        }*/
    }
    public void CollectData()
    {
        if (_currentLevel != 0)
        {

            if (completionTimePerLevel[_currentLevel - 1] == 0)
            {
                completionTimePerLevel[_currentLevel - 1] = stats.GetCompletionTime();
                starsPerLevel[_currentLevel - 1] = stats.GetStars();
                overallTimePerLevel[_currentLevel - 1] = stats.GetOverallTime();
                retryPerLevel[_currentLevel - 1] = stats.GetRetrial();
                paradoxPerLevel[_currentLevel - 1] = stats.GetParadoxes();
            }
            else if (stats.GetStars() >= starsPerLevel[_currentLevel - 1] && stats.GetCompletionTime() < completionTimePerLevel[_currentLevel - 1])
            {
                completionTimePerLevel[_currentLevel - 1] = stats.GetCompletionTime();
                starsPerLevel[_currentLevel - 1] = stats.GetStars();
                overallTimePerLevel[_currentLevel - 1] = stats.GetOverallTime();
                retryPerLevel[_currentLevel - 1] = stats.GetRetrial();
                paradoxPerLevel[_currentLevel - 1] = stats.GetParadoxes();
            }

            if (_currentLevel > _levelsFinished)
            {
                _levelsFinished = _currentLevel;
            }
            //Debug.Log(stats.GetCompletionTime());
            SaveData();
        }
        if (_currentLevel == 1)
        {
            firstTimePlaying = false;
        }

    }

    public void EndedStoryboard()
    {
        if(fromGallery==true){
            fromGallery=false;
            PlayMainMenu();
        }else{
            if(_currentLevel==32){
            PlayLevel(1);
            }else if(_currentLevel==33){
            PlayLevel(34);
            }
        }
        
    }

    public void GoToStoryBoardIntro()
    {
        fromGallery = true;
        if (!firstTimePlaying)
        {
            PlayLevel(32);
        }
    }
    public void GoToStoryBoardOutro()
    {
        fromGallery = true;
        if (!firstTimePlaying)
        {
            PlayLevel(33);
        }
    }

    public bool GetFirstTimePlay()
    {
        return firstTimePlaying;
    }
    public void NewGame()
    {
        starsPerLevel = new int[numOfLevels];
        completionTimePerLevel = new float[numOfLevels];
        overallTimePerLevel = new float[numOfLevels];
        paradoxPerLevel = new int[numOfLevels];
        retryPerLevel = new int[numOfLevels];
        _levelsFinished = -1;
        for (int i = 0; i < numOfLevels; i++)
        {
            starsPerLevel[i] = 0;
            completionTimePerLevel[i] = 0f;
            overallTimePerLevel[i] = 0f;
            retryPerLevel[i] = 0;
            paradoxPerLevel[i] = 0;
        }
        SaveSystem.DestroyData();
        firstTimePlaying = true;
        PlayFirstLevel();
    }

    public bool GetFromGallery(){
        return fromGallery;
    }
    
}




