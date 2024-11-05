using UnityEngine;

public static class LaserUtil
{
    static public Vector2 ToBoundary(Vector2 position, Vector2 direction, float magnitude)
    {
        Vector2 result = new Vector2();
        Vector2 offset = direction.normalized * magnitude;
        result = position + offset;
        return result;
    }
}
