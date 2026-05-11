using Characters.Health;
using UnityEngine;

namespace Combat
{
    public interface ICombat
    {
        IDamageable Self { get; }
        IDamageable Target { get; }
        Transform TargetTransform { get; }
        CharacterDefense TargetDefense { get; }
        void SetTarget(IDamageable target);
    }
}
