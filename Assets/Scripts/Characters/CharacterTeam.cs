using UnityEngine;

public class CharacterTeam : MonoBehaviour
{
    [SerializeField] private Team team;
    public bool IsHostileTo(CharacterTeam other) => other != null && other.team != team;
}