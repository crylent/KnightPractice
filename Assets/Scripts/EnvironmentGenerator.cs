using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentGenerator : MonoBehaviour
{
    [SerializeField] private Rect worldSize;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private uint objectsNumber;
    [SerializeField] private float minimumDistanceBetweenTwoObjects;

    private readonly List<Vector3> _positions = new();

    private void Start()
    {
        for (uint i = 0; i < objectsNumber; i++)
        {
            var position = Utility.RandomPosition(worldSize);
            if (!CheckDistance(position))
            {
                i--; continue;
            }

            var objIndex = Random.Range(0, objects.Length);
            var obj = objects[objIndex];
            Instantiate(obj, position, obj.transform.rotation);
            
            _positions.Add(position);
        }
    }

    private bool CheckDistance(Vector3 position)
    {
        // check if the new object's position meets the minimum distance condition to all existing objects 
        return _positions.All(other => !((position - other).magnitude < minimumDistanceBetweenTwoObjects));
    }
}
