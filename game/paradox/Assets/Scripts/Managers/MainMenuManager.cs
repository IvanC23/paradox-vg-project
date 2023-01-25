using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject selectLevelMenu;
    public GameObject mainMenu;
    public GameObject feedbackMenu;
    public GameObject optionsMenu;
    public GameObject soundsMenu;
    public GameObject resolutionMenu;
    public GameObject optionsButton;
    public GameObject levelButton;
    public GameObject startButton;
    public GameObject feedbackButton;
    public GameObject soundsButton;
    public GameObject resolutionButton;
    public GameObject galleryButton;
    public GameObject newGameButton;

    private bool _isWaiting;
    private void Start()
    {
        mainMenu.SetActive(true);
        selectLevelMenu.SetActive(false);
        feedbackMenu.SetActive(false);
        optionsMenu.SetActive(false);
        resolutionMenu.SetActive(false);
        soundsMenu.SetActive(false);
        if (LevelManager.Instance.GetFirstTimePlay())
        {
            galleryButton.GetComponent<Button>().interactable = false;
            newGameButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            galleryButton.GetComponent<Button>().interactable = true;
            newGameButton.GetComponent<Button>().interactable = true;
        }
    }

    public void PlayGame()
    {
        if (!_isWaiting)
        {
            _isWaiting = true;
            LevelManager.Instance.PlayFirstLevel();
        }
    }

    public void SelectLevel()
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(levelButton);
        
        mainMenu.SetActive(false);
        selectLevelMenu.SetActive(true);
    }

    public void MainMenu()
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(startButton);
        
        mainMenu.SetActive(true);
        selectLevelMenu.SetActive(false);
        feedbackMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void BackToOptions(GameObject menu)
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(optionsButton);
        
        menu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    
    public void Activate(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void Sounds()
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(soundsButton);
        
        optionsMenu.SetActive(false);
        soundsMenu.SetActive(true);
    }

    public void Resolutions()
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(resolutionButton);
        
        optionsMenu.SetActive(false);
        resolutionMenu.SetActive(true);
    }
    
    public void FeedBack()
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(feedbackButton);
        
        mainMenu.SetActive(false);
        feedbackMenu.SetActive(true);
    }
    
    public void Options()
    {
        //Clear
        EventSystem.current.SetSelectedGameObject(null);
        //Reassign
        EventSystem.current.SetSelectedGameObject(optionsButton);
        
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        
    }

    public void Gallery()
    {
        if (!_isWaiting)
        {
            _isWaiting = true;
            LevelManager.Instance.GoToStoryBoard();
        }
    }

    public void NewGame(){
        LevelManager.Instance.NewGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    
}
