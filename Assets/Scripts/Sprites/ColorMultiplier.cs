using System.Collections.Generic;
using UnityEngine;

namespace Sprites
{
    public class ColorMultiplier: MonoBehaviour
    {
        [SerializeField] private Color multiplier = Color.white;
        private readonly Dictionary<SpriteRenderer, Color> _sprites = new(); // sprites with their original colors
        
        protected void Start()
        {
            var rootSprite = GetComponent<SpriteRenderer>();
            if (rootSprite != null) _sprites.Add(rootSprite, rootSprite.color); // add sprite from root if exists

            foreach (var sprite in GetComponentsInChildren<SpriteRenderer>()) // add sprites from children
            {
                _sprites.Add(sprite, sprite.color);
            }
        }
        
        protected void Update()
        {
            foreach (var sprite in _sprites)
            {
                sprite.Key.color = sprite.Value * multiplier;
            }
        }
    }
}