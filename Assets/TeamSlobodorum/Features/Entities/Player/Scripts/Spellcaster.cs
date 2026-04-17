using System;
using JetBrains.Annotations;
using TeamSlobodorum.Spells;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Entities.Player
{
    public class Spellcaster : MonoBehaviour
    {
        public event Action<Spell> CurrentSpellChanged;

        private InputAction _attackAction;

        private Spell[] _spells;
        private int _currentSpellIndex;

        [CanBeNull] public Spell CurrentSpell => _currentSpellIndex >= 0 && _currentSpellIndex < _spells.Length
            ? _spells[_currentSpellIndex]
            : null;

        private void Start()
        {
            _spells = GetComponents<Spell>();
            _currentSpellIndex = 0;
            CurrentSpellChanged?.Invoke(CurrentSpell);
        }

        public void OnAttack()
        {
            CurrentSpell?.Use();
        }

        public void OnCancel()
        {
            CurrentSpell?.CancelEffect();
        }

        public void OnNext()
        {
            if (_currentSpellIndex >= _spells.Length - 1)
            {
                _currentSpellIndex = 0;
            }
            else
            {
                _currentSpellIndex++;
            }

            CurrentSpellChanged?.Invoke(CurrentSpell);
        }

        public void OnPrevious()
        {
            if (_currentSpellIndex == 0)
            {
                _currentSpellIndex = _spells.Length - 1;
            }
            else
            {
                _currentSpellIndex--;
            }

            CurrentSpellChanged?.Invoke(CurrentSpell);
        }
    }
}