using DG.Tweening;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject controlsPanel;

    public void OpenMainPanel()
    {
        gameUI.SetActive(false);
        finishPanel.SetActive(false);

        mainPanel.SetActive(true);
    }

    public void OpenControlsPanel()
    {
        bool state = controlsPanel.activeInHierarchy;
        controlsPanel.SetActive(!state);
    }

    public void StartGame()
    {
        mainPanel.SetActive(false);
        finishPanel.SetActive(false);

        gameUI.SetActive(true);
    }

    public void FinishGame()
    {
        gameUI.SetActive(false);

        finishPanel.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    private void FadePanel(CanvasGroup canvas, float alphaStart, float alphaEnd, float time)
    {
        DOVirtual.Float(alphaStart, alphaEnd, time, x => canvas.alpha = x);
    }
}
