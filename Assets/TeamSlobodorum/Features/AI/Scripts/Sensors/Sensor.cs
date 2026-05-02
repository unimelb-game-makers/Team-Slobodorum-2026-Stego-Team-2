using UnityEngine;

namespace TeamSlobodorum.AI.Sensors
{
    public abstract class Sensor : ScriptableObject
    {
        public abstract void Initialize(Brain brain);
        
        public abstract void Tick(Brain brain);
    }
}