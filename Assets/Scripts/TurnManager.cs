using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_Text playerTurnText;
    public TMP_Text P1Coins;
    public TMP_Text P2Coins;
    private int currentTurn = 0;

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
        //initialize coin values
        P1Coins.text = "10";
        P2Coins.text = "10";
        //make P2coin value invisible
        MakeTextInvisible(P2Coins);
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
                    //update turn counter
                    currentTurn++;
                    //update player coins
                    P1Coins.text = (int.Parse(P1Coins.text) + (currentTurn * 5)).ToString();
                    P2Coins.text = (int.Parse(P2Coins.text) + (currentTurn * 5)).ToString();
                    //show player coins
                    MakeTextVisible(P1Coins);
                    MakeTextInvisible(P2Coins);

                    break;
                case GameState.P2Turn:
                    Debug.Log("Player 2's turn");
                    UpdatePlayerTurnText(GameState.P2Turn);
                    previousState = gameManager.state;
                    //show player coins
                    MakeTextVisible(P2Coins);
                    MakeTextInvisible(P1Coins);
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

    private void MakeTextInvisible(TMP_Text textMeshPro)
    {
        if (textMeshPro != null)
        {
            // Get the current color
            Color textColor = textMeshPro.color;

            // Set alpha to 0 (fully transparent)
            textColor.a = 0f;

            // Apply the modified color back to the TextMeshPro component
            textMeshPro.color = textColor;
        }
        else
        {
            Debug.LogError("TextMeshPro component not assigned!");
        }
    }

    void MakeTextVisible(TMP_Text textMeshPro)
    {
        if (textMeshPro != null)
        {
            // Get the current color
            Color textColor = textMeshPro.color;

            // Set alpha to 1 (fully opaque)
            textColor.a = 1f;

            // Apply the modified color back to the TextMeshPro component
            textMeshPro.color = textColor;
        }
        else
        {
            Debug.LogError("TextMeshPro component not assigned!");
        }
    }
}
