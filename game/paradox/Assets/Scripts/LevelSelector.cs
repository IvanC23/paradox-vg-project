using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelPrefab;
    private TMP_Text levelText;
    private int _totalLevels;
    private int _levelsFinished;
    [SerializeField]
    private bool _isAdmin;

    private int[] _starsPerLevel;
    private int _numberOfExtraLevels=3;
    private bool _isWaiting = false;
    [SerializeField] private LevelNamesParameters levelNames;

    private void Start()
    {
        _totalLevels = SceneManager.sceneCountInBuildSettings-_numberOfExtraLevels;
        _levelsFinished = LevelManager.Instance.getLevelsFinished();
        _starsPerLevel = LevelManager.Instance.GetStarsPerLevel();

        for (int level = 1; level <= _totalLevels; level++)//Hard coded build index start level
        {
            GameObject newButton = Instantiate(levelPrefab, this.transform, false);
            newButton.GetComponentInChildren<TMP_Text>().SetText((level-1).ToString());
            newButton.GetComponentsInChildren<TMP_Text>()[1].SetText(levelNames.levelNames[level - 1]);
            int sceneIndex = level;
            if (_isAdmin)
            {
                newButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                if (_levelsFinished + 1 >= sceneIndex )
                {
                    newButton.GetComponent<Button>().interactable = true;
                    newButton.transform.GetChild(_starsPerLevel[level-1]).gameObject.SetActive(true);
                }
                else
                {
                    newButton.GetComponent<Button>().interactable = false;
                }
            }

            newButton.GetComponent<Button>().onClick.AddListener(() => PlayLevel(sceneIndex));
        }
    }

    private void PlayLevel(int sceneIndex)
    {
        if (!_isWaiting)
        {
            _isWaiting = true;
            LevelManager.Instance.PlayLevel(sceneIndex);
        }
    }
    
}
