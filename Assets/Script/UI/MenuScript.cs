using DG.Tweening;
using System.Collections;
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
        StartCoroutine(FadeAndClose(gameUI, false));
        StartCoroutine(FadeAndClose(finishPanel, false));

        StartCoroutine(FadeAndClose(mainPanel, true));
    }

    public void OpenControlsPanel()
    {
        bool state = controlsPanel.activeInHierarchy;

        StartCoroutine(FadeAndClose(controlsPanel, !state));
    }

    public void StartGame()
    {
        StartCoroutine(FadeAndClose(mainPanel, false));
        StartCoroutine(FadeAndClose(finishPanel, false));
        
        StartCoroutine(FadeAndClose(gameUI, true));
    }

    public void FinishGame()
    {
        StartCoroutine(FadeAndClose(gameUI, false));

        StartCoroutine(FadeAndClose(finishPanel, true));
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    private void FadePanel(CanvasGroup canvas, float alphaStart, float alphaEnd, float time)
    {
        DOVirtual.Float(alphaStart, alphaEnd, time, x => canvas.alpha = x);
    }

    private void ScalePanel(Transform panel, Vector3 scaleEnd, float time)
    {
        panel.DOScale(scaleEnd, time);
    }

    IEnumerator FadeAndClose(GameObject panel, bool active)
    {
        if (active)
        {
            FadePanel(panel.GetComponent<CanvasGroup>(), 0, 1, .3f);
        }
        else
        {
            FadePanel(panel.GetComponent<CanvasGroup>(), 1, 0, .3f);
            yield return new WaitForSeconds(.3f);
        }

        panel.SetActive(active);
    }
}
