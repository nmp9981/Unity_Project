using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace AnimTuto
{
    [UpdateInGroup(typeof(PresentationSystemGroup),OrderFirst =true)]
    public partial struct PlayerAnimateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach(var(playerGameObjectPrefab, entity) in SystemAPI.Query<PlayerGameObjectPrefab>().WithNone<PlayerAnimationReference>().WithEntityAccess())
            {
                var newCompanionGameObject = Object.Instantiate(playerGameObjectPrefab.Value);
                var newAnimatorReference = new PlayerAnimationReference
                {
                    Value = newCompanionGameObject.GetComponent<Animator>()
                };
                ecb.AddComponent(entity, newAnimatorReference);
            }
            foreach (var(transform, animatorReference) in SystemAPI.Query<LocalTransform, PlayerAnimationReference>())
            {
                animatorReference.Value.SetBool("IsMoving",true);//애니메이션 활성화
                animatorReference.Value.transform.position = transform.Position;
                animatorReference.Value.transform.rotation = transform.Rotation;
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
