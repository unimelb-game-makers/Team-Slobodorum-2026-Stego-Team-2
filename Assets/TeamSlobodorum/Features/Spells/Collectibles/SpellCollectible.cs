using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TeamSlobodorum.DataPersistence;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Spells.Player;
using TeamSlobodorum.UI.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace TeamSlobodorum.Spells.Collectibles
{
    public class SpellCollectibles : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private List<WorldSpaceTracker> trackers;
        [SerializeField] private SpellDefinition spellDefinition;

        public SpellDefinition SpellDefinition => spellDefinition;
        private bool onCollision = false;

        void Awake()
        {
            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnSaveRequested += SaveData;
                SaveManager.instance.OnLoadRequested += LoadData;
            }
        }
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                RegisterTrackers();
                onCollision = true;
                other.GetComponent<PlayerSpellManager>().collectibles.Add(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UnregisterTrackers();
                onCollision = false;
                other.GetComponent<PlayerSpellManager>().collectibles.Remove(this);

            }
        }
        public void Collected()
        {
            UnregisterTrackers();
            transform.DOScale(Vector3.zero, 1f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
        }


        public void LoadData(GameData data)
        {
            bool alreadyCollected = data.spells.Any(spell => spell.spellID == spellDefinition.name);

            if (alreadyCollected)
            {
                Destroy(gameObject);
            }
        }

        public void SaveData(GameData data)
        {
        }


        public void OnDestroy()
        {
            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnSaveRequested -= SaveData;
                SaveManager.instance.OnLoadRequested -= LoadData;
            }

        }


    }
}
