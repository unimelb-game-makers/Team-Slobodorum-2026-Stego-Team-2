using System;
using System.Collections.Generic;
using DG.Tweening;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Spells.Player;
using TeamSlobodorum.UI.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace TeamSlobodorum.Spells.Collectibles
{
    public class SpellCollectibles: MonoBehaviour
    {
        [SerializeField] private List<WorldSpaceTracker> trackers;
        [SerializeField] private SpellDefinition spellDefinition;

        public SpellDefinition SpellDefinition => spellDefinition;        
        private bool onCollision = false;

        
        private void RegisterTrackers()
        {
            foreach (var tracker in trackers)
            {
                tracker.RegisterComponent();
            }
        }
        private void UnregisterTrackers()
        {
            foreach (var tracker in trackers)
            {
                tracker.UnregisterComponent();
            }
        }
        
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player"))
            {   
                RegisterTrackers();
                onCollision = true;
                other.GetComponent<PlayerSpellManager>().collectible = this;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player"))
            {   
                UnregisterTrackers();
                onCollision = false;
                other.GetComponent<PlayerSpellManager>().collectible = null;

            }
        }
        public void Collected()
        {
            UnregisterTrackers();
            transform.DOScale(Vector3.zero, 1f)
                    .SetEase(Ease.InQuad) 
                    .OnComplete(() => {
                        Destroy(gameObject);
                    });
        }
    }
}
