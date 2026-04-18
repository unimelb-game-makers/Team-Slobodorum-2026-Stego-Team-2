using System;
using TeamSlobodorum.Entities.Player;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace TeamSlobodorum.Core.Behaviour.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Find Player Entity", story: "Find the [PlayerEntity] in the current scene", category: "Action/Find", id: "194a2bc9c4eed6feaf66cf34235b5ced")]
    public partial class FindPlayerEntityAction : Action
    {
        [SerializeReference] public BlackboardVariable<PlayerEntity> playerEntity;

        protected override Status OnStart()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                playerEntity.Value = player.GetComponent<PlayerEntity>();
                return Status.Success;
            }

            return Status.Failure;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}

