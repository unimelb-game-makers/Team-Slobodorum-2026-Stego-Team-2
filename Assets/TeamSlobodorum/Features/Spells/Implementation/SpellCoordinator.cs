using System.Collections.Generic;
using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public class SpellCoordinator : MonoBehaviour, ISpellCoordinator
    {
        [Header("Services")]

        [SerializeField] private

        SpellFactory _factory;
        AimTargetingService _targetingService = new();
        private readonly Dictionary<int, ISpellRuntime> _activeSpells = new();
        private int _nextHandle = 1;

        private void Awake()
        {
            _factory = new SpellFactory();
        }

        public bool TryCast(SpellCastRequest request, out SpellHandle handle)
        {
            handle = default;

            if (request.Definition == null || request.Caster == null || request.CastOrigin == null)
                return false;

            handle = new SpellHandle(_nextHandle++);

            ISpellRuntime runtime = _factory.Create(request.Definition, handle);
            if (runtime == null)
                return false;

            var context = new SpellContext(
                request.Caster,
                request.CastOrigin,
                request.AimOrigin,
                request.AimDirection,
                request.PlayerOrigin,
                this,
                _targetingService,
                null,
                null
            );

            _activeSpells.Add(handle.Value, runtime);
            runtime.Begin(context);
            return true;
        }

        public bool TryGetRuntime(SpellHandle handle, out ISpellRuntime runtime)
        {
            return _activeSpells.TryGetValue(handle.Value, out runtime);
        }

        public void Cancel(SpellHandle handle, SpellCancelReason reason)
        {
            if (_activeSpells.TryGetValue(handle.Value, out var runtime))
                runtime.Cancel(reason);
        }

        public void Publish(ISpellEvent evt)
        {

        }

        private void Update()
        {
            if (_activeSpells.Count == 0)
                return;

            List<int> finished = null;

            foreach (var kvp in _activeSpells)
            {
                kvp.Value.Tick(Time.deltaTime);

                if (kvp.Value.IsFinished)
                {
                    finished ??= new List<int>();
                    finished.Add(kvp.Key);
                }
            }

            if (finished == null)
                return;

            for (int i = 0; i < finished.Count; i++)
                _activeSpells.Remove(finished[i]);
        }

        private void FixedUpdate()
        {
            foreach (var kvp in _activeSpells)
                kvp.Value.FixedTick(Time.fixedDeltaTime);
        }
    }
}