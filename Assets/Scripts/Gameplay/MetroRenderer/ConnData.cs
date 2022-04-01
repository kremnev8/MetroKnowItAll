using Gameplay;
using UnityEngine;

public struct ConnData
{
    public Vector2 point;
    public MetroConnection connection;

    public ConnData(Vector2 point, MetroConnection connection)
    {
        this.point = point;
        this.connection = connection;
    }
}