using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        WAITINGTOSTART,
        GAMEPLAY,
        LOSE
    }

    private GameState currentState;

    private void Awake()
    {
        Instance = this;
        currentState = GameState.GAMEPLAY;
    }
    private void Update()
    {
        switch (currentState)
        {
            case GameState.WAITINGTOSTART:
                break;
            case GameState.GAMEPLAY:
                break;
            case GameState.LOSE:
                Debug.Log("game ended");
                //Time.timeScale = 0f;
                break;
            default:
                break;
        }
    }

    public void StartGame()
    {
        currentState = GameState.GAMEPLAY;
    }

    public void EndGame()
    {
        currentState = GameState.LOSE;
    }
}
