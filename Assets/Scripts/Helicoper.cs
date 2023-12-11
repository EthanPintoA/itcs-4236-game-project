using UnityEngine;

public class Helicopter : IPiece
{
    public int GetDamage()
    {
        int[] possibleDamages = { 1, 2, 3 };
        int randomIndex = Random.Range(0, possibleDamages.Length);
        return possibleDamages[randomIndex] + Mathf.FloorToInt(DamageBonus / 2f);
    }

    public GameObject GameObject { get; }
    public PieceType Type { get; }
    public int Movement { get; }
    public int Range { get; set; }
    public int Health { get; set; }

    public bool CarryingAnotherPiece { get; set; }

    /// <summary>
    /// Should be 0 if not carrying another piece or the damage bonus of the carried piece.
    /// </summary>
    public int DamageBonus { get; set; }

    // Will add the damage bonus to the damage stat if the damage bonus is greater than 0.
    public string DamageAsStat => "1~3" + ((DamageBonus > 0) ? "+" + DamageBonus : "");

    public Helicopter(GameObject gameObject, PieceType type)
    {
        GameObject = gameObject;
        Type = type;
        Movement = 2;
        Range = 2;
        Health = 3;

        CarryingAnotherPiece = false;
        DamageBonus = 0;
    }
}
