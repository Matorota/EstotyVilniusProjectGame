using UnityEngine;

namespace Characters.Team
{
    public enum TeamId { Player, Enemy, Neutral }

    public class Team : MonoBehaviour
    {
        [SerializeField] private TeamId team;
        public bool IsHostileTo(Team other) => other != null && other.team != team;
    }
}
