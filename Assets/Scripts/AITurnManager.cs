using UnityEngine;
using TMPro;

public class AITurnManager : MonoBehaviour
{
    public AIGameManager gameManager;
    public TMP_Text playerTurnText;
<<<<<<< Updated upstream
=======
    public TMP_Text P1Coins;
    public TMP_Text P2Coins;
    public Canvas P1canvas;
    public Canvas P2canvas;
    private int currentTurn = 0;
>>>>>>> Stashed changes

    // To keep track when the state changes
    // FIXME: GameState management need be done in a better way
    // so we don't need this

<<<<<<< Updated upstream
    GameState previousState;
=======
    //Can we just call UpdatePlayerTurnText whenever we call GetSwitchPlayerTurns
    PlayerTurn previousTurn;
>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        previousState = gameManager.state;
        UpdatePlayerTurnText(gameManager.state);
=======
        UpdatePlayerTurnText(gameManager.playerTurn);
        previousTurn = gameManager.playerTurn;

        //initialize coin values
        P1Coins.text = "10";
        P2Coins.text = "10";
        //make P2coin value invisible
        MakeTextInvisible(P2Coins);

        P2canvas.enabled = false;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        // If the state changed
<<<<<<< Updated upstream
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
=======
        if (gameManager.playerTurn != previousTurn)
        {
            switch (gameManager.playerTurn)
            {
                case PlayerTurn.Player1:
                    Debug.Log("Player 1's turn");
                    UpdatePlayerTurnText(PlayerTurn.Player1);
                    previousTurn = gameManager.playerTurn;
                    //update turn counter
                    currentTurn++;
                    //update player coins
                    P1Coins.text = (int.Parse(P1Coins.text) + (currentTurn * 5)).ToString();
                    P2Coins.text = (int.Parse(P2Coins.text) + (currentTurn * 5)).ToString();
                    //show player coins
                    MakeTextVisible(P1Coins);
                    MakeTextInvisible(P2Coins);

                    //switch to P1 Canvas
                    P2canvas.enabled = false;
                    P1canvas.enabled = true;
                    break;

                case PlayerTurn.Player2:
                    Debug.Log("Player 2's turn");
                    UpdatePlayerTurnText(PlayerTurn.Player2);
                    previousTurn = gameManager.playerTurn;
                    //show player coins
                    MakeTextVisible(P2Coins);
                    MakeTextInvisible(P1Coins);

                    //switch to P2 Canvas
                    P1canvas.enabled = false;
                    P2canvas.enabled = true;
>>>>>>> Stashed changes
                    break;
            }
        }
    }

<<<<<<< Updated upstream
    private void UpdatePlayerTurnText(GameState playerTurn)
    {
        playerTurnText.text = playerTurn switch
        {
            GameState.P1Turn => "Player 1's\nTurn",
            GameState.P2Turn => "Player 2's\nTurn",
            _ => playerTurnText.text
        };
    }
=======
    private void UpdatePlayerTurnText(PlayerTurn playerTurn)
    {
        playerTurnText.text = playerTurn switch
        {
            PlayerTurn.Player1 => "Player 1's\nTurn",
            PlayerTurn.Player2 => "Player 2's\nTurn",
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
>>>>>>> Stashed changes
}
