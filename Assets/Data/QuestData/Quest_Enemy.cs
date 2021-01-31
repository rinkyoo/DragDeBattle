using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Quest",menuName="Create_Quest")]
public class Quest_Enemy : ScriptableObject
{

    

    [SerializeField]
    private List<Enemy_Wave> enemyWave = new List<Enemy_Wave>();

    [SerializeField] private string questName;
    public string QuestName
    {
        get{ return questName; }
    }

    public List<Enemy_Wave> GetWaveList()
    {
        return enemyWave;
    }
    public Enemy_Wave GetWave(int i)
    {
        return enemyWave[i];
    }
}
