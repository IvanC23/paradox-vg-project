using System;
using Unity.VisualScripting;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private float _masterSlider = 1f;
    private float _effectSlider = 1f;
    private float _themeSlider = 1f;
    
    private int _resolutionDropdownIndex = 2;
    private bool _isFullScreen = true;
    private bool _colorBlindMode = false;

    public static AudioManager instance;
    private bool _isUsingJetpack;
    private bool _wasUsingJetpack;
    private ThemeMusic _currentThemeMusic;
    
    private bool _isUsingKeyboard=true;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    
    /*
     * Used only for the theme sound
     */
    private void Start()
    {
        
        Play("ThemeMenu");
        _currentThemeMusic = ThemeMusic.Menu;
        SceneManager.activeSceneChanged += ChangedActiveScene;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

    }
    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (_isUsingJetpack)
        {
            Stop("Jetpack");
            _isUsingJetpack = false;
            _wasUsingJetpack = true;
            return;
        }

        if (_wasUsingJetpack&& state==GameState.OldPlayerTurn&& GameManager.Instance.PreviousGameState==GameState.PauseMenu)
        {
            Play("Jetpack");
            _isUsingJetpack = true;
            _wasUsingJetpack = false;
            return;
        }
        _wasUsingJetpack = false;
        
    }
    private void ChangedActiveScene(Scene current, Scene next)
    {
        Stop("Jetpack");
        if (next.buildIndex == 0 )
        {
            StopAllTheme();
            Play("ThemeMenu");
            _currentThemeMusic = ThemeMusic.Menu;
            Debug.Log("ThemeMenu");
        }
        if (next.buildIndex == 32 )
        {
            StopAllTheme();
            Play("ThemeStory");
            _currentThemeMusic = ThemeMusic.Story;
            Debug.Log("ThemeStory");
        }
        if (next.buildIndex == 33 )
        {
            StopAllTheme();
            Play("ThemeEnd");
            _currentThemeMusic = ThemeMusic.End;
            Debug.Log("ThemeEnd");
        }

        if (next.buildIndex >=1&&next.buildIndex<11&&_currentThemeMusic!=ThemeMusic.Lab1)
        {
            StopAllTheme();
            Play("ThemeLab1"); 
            _currentThemeMusic = ThemeMusic.Lab1;
            Debug.Log("ThemeLab1");
        }
        if (next.buildIndex >=11&&next.buildIndex<21&&_currentThemeMusic!=ThemeMusic.Lab2)
        {
           
            StopAllTheme(); 
            Play("ThemeLab2"); 
            _currentThemeMusic = ThemeMusic.Lab2;
            Debug.Log("ThemeLab2"); 
        }
        if (next.buildIndex >=21&&next.buildIndex<=31&&_currentThemeMusic!=ThemeMusic.Lab3)
        {
            StopAllTheme(); 
            Play("ThemeLab3"); 
            _currentThemeMusic = ThemeMusic.Lab3;
            Debug.Log("ThemeLab3");

        }
    }
    
    public void Play(string name)
    {
        Sound s = Array.Find(sounds,sound=>sound.name == name);
        if (s == null)
        {
            //Debug.LogWarning("Sounds: "+ name+ " not found!");
            return;
        }
        s.source.Play();
        if (name == "Jetpack")
        {
            _isUsingJetpack = true;
            _wasUsingJetpack = false;
        }
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds,sound=>sound.name == name);
        if (s == null)
        {
            //Debug.LogWarning("Sounds: "+ name+ " not found!");
            return;
        }
        s.source.Stop();
        if (name == "Jetpack")
        {
            _isUsingJetpack = false;
            _wasUsingJetpack = false;
        }
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds,sound=>sound.name == name);
        if (s == null)
        {
            //Debug.LogWarning("Sounds: "+ name+ " not found!");
            return;
        }
        s.source.Pause();
    }

    public void Resume(string name)
    {
        Sound s = Array.Find(sounds,sound=>sound.name == name);
        if (s == null)
        {
            //Debug.LogWarning("Sounds: "+ name+ " not found!");
            return;
        }
        s.source.UnPause();
    }

    public void SetEffectVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            if (s.name != "ThemeMenu"&&s.name != "ThemeStory"&&s.name != "ThemeEnd"&&s.name != "ThemeLab1"&&s.name != "ThemeLab2"&&s.name != "ThemeLab3")
            {
                s.source.volume = s.volume * volume;
            }
        }

        _effectSlider = volume;
    }

    public void SetThemeVolume(float volume)
    {
        Sound s = Array.Find(sounds,sound=>sound.name == "ThemeMenu");
        s.source.volume = s.volume * volume;
        s = Array.Find(sounds,sound=>sound.name == "ThemeStory");
        s.source.volume = s.volume * volume;
        s = Array.Find(sounds,sound=>sound.name == "ThemeEnd");
        s.source.volume = s.volume * volume;
        s = Array.Find(sounds,sound=>sound.name == "ThemeLab1");
        s.source.volume = s.volume * volume;
        s = Array.Find(sounds,sound=>sound.name == "ThemeLab2");
        s.source.volume = s.volume * volume;
        s = Array.Find(sounds,sound=>sound.name == "ThemeLab3");
        s.source.volume = s.volume * volume;
        _themeSlider = volume;
    }
    public void SetMasterVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * volume;
        }

        _masterSlider = volume;
        //Debug.Log("Master volume: " + volume);
    }

    public float GetEffectVolume()
    {
        return _effectSlider;
    }
    public float GetThemeVolume()
    {
        Sound s = Array.Find(sounds,sound=>sound.name == "Theme");
        return _themeSlider;
    }
    public float GetMasterVolume()
    {
        Sound s = Array.Find(sounds,sound=>sound.name == "Theme");
        return +_masterSlider;
    }

    public int getResolutionIndex()
    {
        return _resolutionDropdownIndex;
    }
    
    public void setResolutionIndex(int index)
    {
        _resolutionDropdownIndex = index;
    }

    public bool getFullScreen()
    {
        return _isFullScreen;
    }

    public void setFullScreen(bool isfullscreen)
    {
        _isFullScreen = isfullscreen;
    }

    private void StopAllTheme()
    {
        Stop("ThemeMenu");
        Stop("ThemeStory");
        Stop("ThemeEnd");
        Stop("ThemeLab1");
        Stop("ThemeLab2");
        Stop("ThemeLab3");
    }

    public void SetIsUsingKeyboard(bool i)
    {
        _isUsingKeyboard = i;
        
    }

    public void SetColorBindMode(bool newColorBlindMode)
    {
        _colorBlindMode = newColorBlindMode;
        Debug.Log("Color Blind Mode is On" + _colorBlindMode);
    }

    public bool GetColorBlindMode()
    {
        return _colorBlindMode;
    }

    public bool GetIsUsingKeyboard()
    {
        return _isUsingKeyboard;
    }
}

public enum ThemeMusic
{
    Menu,
    Story,
    End,
    Lab1,
    Lab2,
    Lab3
}

