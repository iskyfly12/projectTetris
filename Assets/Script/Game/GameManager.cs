using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseBackground;

    private BoardBehaviour boardBehaviour;
    private Score scoreBehaviour;
    private MenuScript menuScript;

    void Awake()
    {
        boardBehaviour = FindObjectOfType<BoardBehaviour>();
        scoreBehaviour = FindObjectOfType<Score>();
        menuScript = FindObjectOfType<MenuScript>();
    }

    public void StartNewGame()
    {
        pauseBackground.SetActive(false);
        boardBehaviour.Init(IncreaseScore, LoseGame);
        scoreBehaviour.Init();
    }

    public void LoseGame()
    {
        menuScript.FinishGame();
        boardBehaviour.enabled = false;
    }

    public void PauseGame()
    {
        bool isActive = pauseBackground.activeInHierarchy;

        pauseBackground.SetActive(!isActive);
        boardBehaviour.enabled = isActive;
    }

    void IncreaseScore(int level, int numLines)
    {
        scoreBehaviour.AddScore(level, numLines);
    }
}
