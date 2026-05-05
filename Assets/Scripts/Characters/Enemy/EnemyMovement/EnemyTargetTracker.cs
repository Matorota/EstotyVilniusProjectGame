using UnityEngine;

public class EnemyTargetTracker : MonoBehaviour
{
    [SerializeField] CharacterMovements targetCharacter;

    public Transform ResolveTarget()
    {
        return targetCharacter != null ? targetCharacter.transform : null;
    }
}
