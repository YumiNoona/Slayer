using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }

    protected void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }

    protected void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }

}
