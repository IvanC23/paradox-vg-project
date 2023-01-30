using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsHandler : MonoBehaviour
{
    public Dropdown m_Dropdown;
    public List<ResItem> resolution = new List<ResItem>();
    private int selectedResolution;
    public Toggle fullscreenToggle;
    public Toggle colorBlindToggle;
    private bool _colorBlindMode;

    private void Awake()
    {
        m_Dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(m_Dropdown); });
        colorBlindToggle.onValueChanged.AddListener(delegate { OnColorBlindToggleValueChanged(colorBlindToggle); });
        colorBlindToggle.isOn = AudioManager.instance.GetColorBlindMode();
        
        fullscreenToggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(fullscreenToggle); });
        fullscreenToggle.isOn = AudioManager.instance.getFullScreen();
        int index = AudioManager.instance.getResolutionIndex();
        m_Dropdown.value = index;
    }

    public void DropdownValueChanged(Dropdown change)
    {
        selectedResolution = change.value;
        setResolution(change.value);
    }

    public void setResolution(int index)
    {
        if (AudioManager.instance.getFullScreen())
        {
            Screen.SetResolution(resolution[index].horizontal, resolution[index].vertical,
                FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(resolution[index].horizontal, resolution[index].vertical,
                FullScreenMode.Windowed);
        }

        AudioManager.instance.setResolutionIndex(index);
    }

    public void OnToggleValueChanged(Toggle change)
    {
        fullscreenToggle.isOn = change.isOn;
        FullScreene(fullscreenToggle.isOn);
    }

    public void OnColorBlindToggleValueChanged(Toggle change)
    {
        colorBlindToggle.isOn = change.isOn;
        AudioManager.instance.SetColorBindMode(colorBlindToggle.isOn);
        
    }

    public void FullScreene(bool isOn)
    {
        if (isOn)
        {
            Screen.fullScreen = true;
            AudioManager.instance.setFullScreen(true);
        }
        else
        {
            Screen.fullScreen = false;
            AudioManager.instance.setFullScreen(false);
        }
    }

    public void SetColorBindMode(bool _colorBlindMode)
    {
        
    }
}

[System.Serializable]public class ResItem
{
    public int horizontal, vertical;
}