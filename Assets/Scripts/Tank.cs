using UnityEngine;

public class Tank : IPiece
{
    public static int movement = 1;
    // public int GetDamage()
    // {
    //     Random random = new Random();
    //     int randomNumber = random.Next(1, 101);
    //     return (randomNumber <= 50) ? 3 : 4;
    // }
    public int GetDamage()
    {
        int randomNumber = UnityEngine.Random.Range(1, 101);
        return (randomNumber <= 50) ? 3 : 4;
    }
    public static int health = 5;
    public static int range = 2;
    public GameObject GameObject { get; }

    public Tank(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
