using UnityEngine;

namespace Utility
{
    internal class AddressablesController: MonoBehaviour
    {
        internal static AddressablesController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}