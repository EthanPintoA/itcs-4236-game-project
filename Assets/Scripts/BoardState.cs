using System.Linq;

using UnityEngine;

public class BoardState : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    [Tooltip("The GameObject that contains all the walls in the scene.")]
    private GameObject wallsParent;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefab;

    // 10 x 10 board
    private readonly IPiece[] pieces = new IPiece[100];

    void Awake()
    {
        var wallsArray = wallsParent.transform.GetComponentsInChildren<Transform>();

        // Skip the first child, which is the parent itself.
        foreach (var child in wallsArray.Skip(1))
        {
            var (x, y) = GlobalPositionToPiecePosition(child.position.x, child.position.y);

            SetPiece(new Wall(), x, y);
        }

        foreach (var (piece, i) in pieces.Select((p, i) => (p, i)))
        {
            if (pieces[i] != null)
            {
                continue;
            }

            var x = i % 10;
            var y = i / 10;

            var (globalX, globalY) = PiecePositionToGlobalPosition(x, y);

            var _ = Instantiate(
                SoldierPrefab,
                new Vector3(globalX, globalY, -1f),
                Quaternion.identity
            );
        }
    }

    /// <summary>
    /// Get the piece at the given position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public IPiece GetPiece(int x, int y)
    {
        return pieces[x + (y * 10)];
    }

    /// <summary>
    /// Set the piece at the given position.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public void SetPiece(IPiece piece, int x, int y)
    {
        pieces[x + (y * 10)] = piece;
    }

    /// <summary>
    /// Converts a global position to a piece's position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public (int, int) GlobalPositionToPiecePosition(float x, float y)
    {
        // (-4.5, 4.5) is the position of the top left corner of the board and a Wall is 1 unit wide.
        // Therefore, if a child is at (-4.5, 4.5), it should be at pieces[0].
        // If a child is at (-3.5, 4.5), it should be at pieces[1].
        // If a child is at (-4.5, 3.5), it should be at pieces[10].
        return (Mathf.RoundToInt(x + 4.5f), Mathf.RoundToInt(-y + 4.5f));
    }

    /// <summary>
    /// Converts a piece's position to a global position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public (float, float) PiecePositionToGlobalPosition(int x, int y)
    {
        return (x - 4.5f, -y + 4.5f);
    }
}
