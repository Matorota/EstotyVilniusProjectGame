using UnityEngine;

public class CountEnemies : MonoBehaviour
{
    private Health[] enemyHealthSources = new Health[0];

    public Health[] FindAllEnemies()
    {
        Health[] allHealth = FindObjectsOfType<Health>();
        int enemyCount = 0;

        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemyCount++;
            }
        }

        Health[] enemyHealth = new Health[enemyCount];
        int enemyIndex = 0;
        for (int i = 0; i < allHealth.Length; i++)
        {
            if (allHealth[i].Team == Team.Enemy)
            {
                enemyHealth[enemyIndex] = allHealth[i];
                enemyIndex++;
            }
        }

        enemyHealthSources = enemyHealth;
        return enemyHealth;
    }

    public int CountAliveEnemies()
    {
        int aliveCount = 0;
        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            if (enemyHealthSources[i] != null && enemyHealthSources[i].CurrentHealth > 0f)
            {
                aliveCount++;
            }
        }
        return aliveCount;
    }

    public void SubscribeToDeaths(System.Action onEnemyDeath)
    {
        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            if (enemyHealthSources[i] != null)
            {
                enemyHealthSources[i].OnDeath += onEnemyDeath;
            }
        }
    }

    public void UnsubscribeFromDeaths(System.Action onEnemyDeath)
    {
        if (enemyHealthSources == null) return;

        for (int i = 0; i < enemyHealthSources.Length; i++)
        {
            if (enemyHealthSources[i] != null)
            {
                enemyHealthSources[i].OnDeath -= onEnemyDeath;
            }
        }
    }
}
