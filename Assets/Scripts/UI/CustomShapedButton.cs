using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CustomShapedButton : MonoBehaviour
    {
        [SerializeField] private float alphaThreshold = 0.5f;
        
        // Start is called before the first frame update
        private void Start()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}
