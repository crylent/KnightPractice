using UnityEngine;

namespace Utility
{
    public static class CustomRandom
    {
        public static Vector3 GetPosition(Rect area)
        {
            return new Vector3(
                Random.Range(area.xMin, area.xMax),
                0,
                Random.Range(area.yMin, area.yMax)
            );
        }
    }
}