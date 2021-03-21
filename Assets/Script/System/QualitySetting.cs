using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySetting : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    private bool isPanelActive;

    public void OpenPanel() 
    {
        if (isPanelActive)
            settingPanel.transform.DOScale(Vector3.zero, .2f);
        else
            settingPanel.transform.DOScale(Vector3.one, .3f);

        isPanelActive = !isPanelActive;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
