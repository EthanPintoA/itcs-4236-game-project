using TMPro;
using UnityEngine;

public class PieceStatsManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private BoardManager boardManager;

    [Header("Game Objects")]
    [SerializeField]
    private TMP_Text pieceHealthText;

    [SerializeField]
    private TMP_Text pieceMovementText;

    [SerializeField]
    private TMP_Text pieceRangeText;

    [SerializeField]
    private TMP_Text pieceDamageText;

# nullable enable
    private IPiece? previousPiece;

    void Start()
    {
        previousPiece = null;

        UpdatePieceStats(previousPiece);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.selected == null && previousPiece != null)
        {
            previousPiece = null;
            UpdatePieceStats(previousPiece);
            Debug.Log("Updated piece stats");
            return;
        }
        else if (gameManager.selected != null)
        {
            var piece = boardManager.boardState.GetPiece(gameManager.selected.Value);

            if (piece != null && piece != previousPiece)
            {
                previousPiece = piece;
                UpdatePieceStats(piece);
                Debug.Log("Updated piece stats");
            }
        }
    }

    public void UpdatePieceStats(IPiece? piece)
    {
        if (piece == null)
        {
            pieceHealthText.text = "Health: ~";
            pieceMovementText.text = "Movement: ~";
            pieceRangeText.text = "Range: ~";
            pieceDamageText.text = "Damage: ~";
        }
        else
        {
            pieceHealthText.text = "Health: " + piece.Health;
            pieceMovementText.text = "Movement: " + piece.Movement;
            pieceRangeText.text = "Range: " + piece.Range;
            pieceDamageText.text = "Damage: " + piece.DamageAsStat;
        }
    }
}
