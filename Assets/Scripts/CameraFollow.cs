using Player;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 5f;
    
    private static Vector3 PlayerPosition => PlayerComponents.Transform.position;

    private void LateUpdate()
    {
        var deltaPosition = PlayerPosition - transform.position;
        deltaPosition.z = 0;
        transform.Translate(followSpeed * Time.deltaTime * deltaPosition);
    }
}
