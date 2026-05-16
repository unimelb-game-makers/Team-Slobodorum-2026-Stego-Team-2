using UnityEngine;

namespace TeamSlobodorum.AI.Sensors
{
    public abstract class Sensor : ScriptableObject
    {
        public abstract void OnStart(Brain brain);
        
        public abstract void OnUpdate(Brain brain);
    }
}