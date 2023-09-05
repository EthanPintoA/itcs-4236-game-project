using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    private BoardManager boardManager;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefab;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var (pieceGridPos, pieceGlobalPos) = MousePositionToPiecePosition();

            Debug.Log($"Creating piece at {pieceGridPos}");

            var soldierObj = Instantiate(SoldierPrefab, pieceGlobalPos, Quaternion.identity);
            boardManager.boardState.SetPiece(new Soldier(soldierObj), pieceGridPos);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var (piecePos, _) = MousePositionToPiecePosition();

            Debug.Log($"Deleting piece at {piecePos}");

            boardManager.boardState.SetPiece(null, piecePos);
        }
    }

    /// <summary>
    /// Converts the mouse's position to a piece's position.
    /// </summary>
    /// <returns> The piece's grid position and global position. </returns>
    public (Vector2Int, Vector2) MousePositionToPiecePosition()
    {
        var mouseScreenPos = Mouse.current.position.ReadValue();
        var mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
        var pieceGridPos = boardManager.GlobalPositionToPiecePosition(mouseWorldPos);
        var pieceGlobalPos = boardManager.PiecePositionToGlobalPosition(pieceGridPos);

        return (pieceGridPos, pieceGlobalPos);
    }
}
