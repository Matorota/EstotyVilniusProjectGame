using UnityEngine;

public class EnemyTargetTracker : MonoBehaviour
{
    public Transform ResolveTarget()
    {
        return CharacterMovements.MainCharacter != null ? CharacterMovements.MainCharacter.transform : null;
    }
}
