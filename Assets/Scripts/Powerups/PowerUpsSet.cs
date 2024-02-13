using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps
{
    public class PowerUpsSet: MonoBehaviour
    {
        [Serializable]
        public class Item
        {
            public PowerUp powerUp;
            public float chance;
        }

        public List<Item> powerUps;
    }
}