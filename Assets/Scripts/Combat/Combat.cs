using Characters.Health;
using UnityEngine;

namespace Combat
{
    public class Combat : MonoBehaviour, ICombat
    {
        private IDamageable target;
        [SerializeField] private CharacterDefense targetDefense;
        private IDamageable self;

        public IDamageable Self => self;
        public IDamageable Target
        {
            get
            {
                CleanupDestroyedReferences();
                return target;
            }
        }
        public Transform TargetTransform
        {
            get
            {
                CleanupDestroyedReferences();
                return (target as Component)?.transform;
            }
        }
        public CharacterDefense TargetDefense => targetDefense;

        private void Awake()
        {
            self = GetComponent<IDamageable>() ?? GetComponentInParent<IDamageable>();
            if (target == null && targetDefense != null)
            {
                target = targetDefense.GetComponent<IDamageable>();
            }
            if (targetDefense == null && target is Component targetComponent)
            {
                targetDefense = targetComponent.GetComponent<CharacterDefense>();
            }

        }

        public void SetTarget(IDamageable newTarget)
        {
            if (newTarget == null)
            {
                target = null;
                targetDefense = null;
                CleanupDestroyedReferences();
                return;
            }

            target = newTarget;
            targetDefense = (newTarget as Component)?.GetComponent<CharacterDefense>();
            CleanupDestroyedReferences();
        }

        private void CleanupDestroyedReferences()
        {
            if (target is Object targetObject && targetObject == null)
            {
                target = null;
                targetDefense = null;
            }

            if (targetDefense != null && targetDefense == null)
            {
                targetDefense = null;
            }
        }
    }
}
