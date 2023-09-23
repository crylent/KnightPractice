using System.Collections;
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

    public static void PlayEffectOnce(ParticleSystem effect, MonoBehaviour parent)
    {
        parent.StartCoroutine(PlayEffectOnceAndDestroy(effect, parent.transform));
    }

    private static IEnumerator PlayEffectOnceAndDestroy(ParticleSystem effect, Transform parent)
    {
        var transform = effect.transform;
        var system = Object.Instantiate(effect, parent.position + transform.position, transform.rotation);
        yield return new WaitWhile(system.IsAlive);
        Object.Destroy(system.gameObject);
    }
}
