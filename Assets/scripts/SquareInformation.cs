
using UnityEngine;

public class SquareInformation
{
    public Vector2 Position=Vector2.zero;
    public int Weight=0;

    public SquareInformation(Vector2 position, int weight)
    {
        Position = position;
        Weight = weight;
    }

    public SquareInformation()
    {
        
    }
}
