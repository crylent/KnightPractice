using UnityEngine;

namespace Utility
{
    internal class AddressablesController: MonoBehaviour
    {
        internal static AddressablesController Instance { get; private set; }

        private void Start()
        {
            Instance = this;
        }
    }
}