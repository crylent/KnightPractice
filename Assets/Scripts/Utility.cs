using UnityEngine;

public static class Utility
{
    public static Vector3 RandomPosition(Rect area)
    {
        return new Vector3(
            Random.Range(area.xMin, area.xMax),
            0,
            Random.Range(area.yMin, area.yMax)
        );
    }
}
