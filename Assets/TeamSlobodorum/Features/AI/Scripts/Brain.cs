using System;
using System.Collections.Generic;
using TeamSlobodorum.AI.Memory;
using TeamSlobodorum.AI.Sensors;
using UnityEngine;

namespace TeamSlobodorum.AI
{
    [Serializable]
    internal struct DebugMemoryModule
    {
        public string name;
        public string value;
    }

    public class Brain : MonoBehaviour
    {
        [SerializeField] private List<Sensor> sensors;
#if UNITY_EDITOR
        [SerializeField] private bool debug;
        [SerializeField] private List<DebugMemoryModule> debugMemory;
#endif
        private readonly Dictionary<int, object> _memory = new();

        public void AddMemoryModule<T>(MemoryModuleType<T> type)
        {
            _memory[type.Id] = null;
        }

        public T GetMemoryValue<T>(MemoryModuleType<T> type)
        {
            return (T)_memory[type.Id];
        }

        public T GetMemoryValueOrDefault<T>(MemoryModuleType<T> type)
        {
            return (T)_memory.GetValueOrDefault(type.Id);
        }

        public void RememberMemoryValue<T>(MemoryModuleType<T> type, T value)
        {
            _memory[type.Id] = value;
#if UNITY_EDITOR
            if (debug) RefreshDebugMemory();
#endif
        }

        public void ForgetMemoryValue<T>(MemoryModuleType<T> type)
        {
            _memory.Remove(type.Id);
#if UNITY_EDITOR
            if (debug) RefreshDebugMemory();
#endif
        }

        public void AddSensor(Sensor sensor)
        {
            sensors.Add(sensor);
        }

        private void Start()
        {
            for (var i = 0; i < sensors.Count; i++)
            {
                sensors[i] = Instantiate(sensors[i]);
                sensors[i].Initialize(this);
            }
        }

        private void Update()
        {
            foreach (var sensor in sensors)
            {
                sensor.Tick(this);
            }
        }

#if UNITY_EDITOR
        private void RefreshDebugMemory()
        {
            debugMemory.Clear();
            foreach (var (id, value) in _memory)
            {
                debugMemory.Add(new DebugMemoryModule()
                {
                    name = MemoryModuleTypeRegistry.MemoryModuleName(id),
                    value = value.ToString(),
                });
            }
        }
#endif
    }
}