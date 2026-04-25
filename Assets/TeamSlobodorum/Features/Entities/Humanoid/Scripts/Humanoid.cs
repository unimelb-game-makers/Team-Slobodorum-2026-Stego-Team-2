using UnityEngine;

namespace TeamSlobodorum.Entities.Humanoid
{
    public class Humanoid : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] public Transform rightHand;
        [SerializeField] public GameObject stepRayUpper;
        [SerializeField] public GameObject stepRayLower;
    }
}
