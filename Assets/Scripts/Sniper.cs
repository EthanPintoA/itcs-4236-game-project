using UnityEngine;

public class Sniper : IPiece
{
    public static int movement = 1;
    // public int GetDamage()
    // {
    //     Random random = new Random();
    //     int randomNumber = random.Next(1, 101);
    //     return (randomNumber <= 30) ? 1 :
    //         (randomNumber <= 60) ? 2 :
    //         (randomNumber <= 85) ? 3 :
    //         4;
    // }
    public int GetDamage()
    {
        int randomNumber = UnityEngine.Random.Range(1, 101);
        return (randomNumber <= 30) ? 1 :
            (randomNumber <= 60) ? 2 :
            (randomNumber <= 85) ? 3 :
            4;
    }
    public static int health = 1;
    public static int range = 1;
    public GameObject GameObject { get; }

    public Sniper(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
