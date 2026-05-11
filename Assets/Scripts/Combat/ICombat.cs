using Characters.Health;
using UnityEngine;

namespace Combat
{
    public interface ICombat
    {
        bool HasValidSelf { get; }
        IDamageable Self { get; }
        IDamageable Target { get; }
        Transform TargetTransform { get; }
        CharacterDefense TargetDefense { get; }
        bool IsSelfDefending { get; }
        bool IsTargetDefending { get; }
        void SetTarget(IDamageable target);
        void ClearTarget();
    }
}
