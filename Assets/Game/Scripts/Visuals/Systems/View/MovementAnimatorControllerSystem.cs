﻿using Game.Components;
using Game.Systems;
using Game.Visuals.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Visuals.Systems
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AnimationSystemGroup))]
    public partial class MovementAnimatorControllerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityQuery stateQuery =
                SystemAPI.QueryBuilder().WithAll<MovementState>().WithAll<VisualAnimator>().Build();

            Dependency.Complete();
            var moveStateSetJob = new AnimatorMoveStateSetJob();
            moveStateSetJob.Run(stateQuery);

            EntityQuery speedQuery =
                SystemAPI.QueryBuilder().WithAll<MovementSpeed>().WithAll<VisualAnimator>().Build();

            var speedSetJob = new AnimatorSpeedSetJob();
            speedSetJob.Run(speedQuery);

            EntityQuery directionQuery = SystemAPI.QueryBuilder().WithAll<MovementDirection, LocalTransform>()
                .WithAll<VisualAnimator>().Build();

            var directionSetJob = new AnimatorDirectionSetJob()
                { MovementFlagsLookup = SystemAPI.GetComponentLookup<MovementFlags>() };
            directionSetJob.Run(directionQuery);
        }
    }

    public partial struct AnimatorMoveStateSetJob : IJobEntity
    {
        private void Execute(in MovementState state, VisualAnimator animator)
        {
            animator.Value.SetMovementState(state.Value);
        }
    }

    public partial struct AnimatorSpeedSetJob : IJobEntity
    {
        private void Execute(in MovementSpeed movementSpeed, VisualAnimator animator)
        {
            animator.Value.SetSpeed(movementSpeed.Value);
        }
    }

    public partial struct AnimatorDirectionSetJob : IJobEntity
    {
        public ComponentLookup<MovementFlags> MovementFlagsLookup;

        private void Execute(in MovementDirection direction, in LocalTransform transform, VisualAnimator animator,
            Entity entity)
        {
            if (direction.Value.Equals(float3.zero))
            {
                animator.Value.SetDirection(0, 0);
                return;
            }

            if (MovementFlagsLookup.TryGetComponent(entity, out MovementFlags movementFlags))
            {
                if (movementFlags.IsMoving == false)
                {
                    animator.Value.SetDirection(0, 0);
                    return;
                }
            }

            var vector = new float2(direction.Value.x, direction.Value.z);
            float3 normal = transform.Forward();
            float normalAngle = math.atan2(normal.z, normal.x);
            float directionAngle = math.atan2(vector.y, vector.x) - normalAngle;
            directionAngle += math.PIHALF;
            var newDirection = new float2(math.cos(directionAngle), math.sin(directionAngle));
            animator.Value.SetDirection(newDirection.x, newDirection.y);
        }
    }
}