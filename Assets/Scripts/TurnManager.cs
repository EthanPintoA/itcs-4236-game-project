using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text playerTurnText;

    // To keep track when the state changes
    // FIXME: GameState management need be done in a better way
    // so we don't need this

    //Can we just call UpdatePlayerTurnText whenever we call GetSwitchPlayerTurns
    GameState previousState;

    // Start is called before the first frame update
    void Start()
    {
        previousState = gameManager.state;
        UpdatePlayerTurnText(gameManager.state);
    }

    // Update is called once per frame
    void Update()
    {
        // If the state changed
        if (gameManager.state != previousState)
        {
            switch (gameManager.state)
            {
                case GameState.P1Turn:
                    Debug.Log("Player 1's turn");
                    UpdatePlayerTurnText(GameState.P1Turn);
                    previousState = gameManager.state;

                    break;
                case GameState.P2Turn:
                    Debug.Log("Player 2's turn");
                    UpdatePlayerTurnText(GameState.P2Turn);
                    previousState = gameManager.state;
                    break;
            }
        }
    }

    private void UpdatePlayerTurnText(GameState playerTurn)
    {
        playerTurnText.text = playerTurn switch
        {
            GameState.P1Turn => "Player 1's\nTurn",
            GameState.P2Turn => "Player 2's\nTurn",
            _ => playerTurnText.text
        };
    }
}
