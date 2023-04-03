using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryboardScript : MonoBehaviour
{
    private int counter;
    private int currentIdx = 0;
    public GameObject storyboards;
    private bool _isWaiting = false;

    private Button prevButton;
    private GameObject nextButton;
    private void Awake()
    {
        storyboards.transform.GetChild(0).gameObject.SetActive(true);
        counter = storyboards.transform.childCount;
        prevButton = this.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        nextButton = this.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;

        /*if (currentIdx == 0 && LevelManager.Instance.GetCurrentLevel() == 24 && !LevelManager.Instance.GetFromGallery())
        {
            prevButton.interactable = false;
            
        }
        */ //commented from pc version
    }



    public void Next()
    {
        prevButton.interactable = true;
        currentIdx++;
        if (currentIdx < counter)
        {
            storyboards.transform.GetChild(currentIdx - 1).gameObject.SetActive(false);
            storyboards.transform.GetChild(currentIdx).gameObject.SetActive(true);
        }
        else
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                LevelManager.Instance.EndedStoryboard();
            }

            //LOAD SCENE
            //LevelManager.Instance.PlayFirstLevel();
        }
    }

    public void Previous()
    {
        currentIdx--;
        /*
        if (currentIdx == 0 && LevelManager.Instance.GetCurrentLevel() == 24 && !LevelManager.Instance.GetFromGallery())
        {
            EventSystem.current.SetSelectedGameObject(null);
            prevButton.interactable = false;
            EventSystem.current.SetSelectedGameObject(nextButton);
            
        }*/ //commented from pc versione

        if (currentIdx >= 0)
        {
            storyboards.transform.GetChild(currentIdx + 1).gameObject.SetActive(false);
            storyboards.transform.GetChild(currentIdx).gameObject.SetActive(true);
        }
        else
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                LevelManager.Instance.PlayMainMenu();
            }
            //LOAD MENU

        }
    }
}
