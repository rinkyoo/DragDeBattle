using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Quest",menuName="Create_Quest")]
public class Quest_Data : ScriptableObject
{
    [SerializeField] private List<Enemy_Wave> enemyWave = new List<Enemy_Wave>();

    [SerializeField] private string questName;
    public string QuestName
    {
        get{ return questName; }
    }

    public List<Enemy_Wave> GetWaveList()
    {
        return enemyWave;
    }

    [SerializeField] private int clearExp;
    public int ClearExp
    {
        get { return clearExp; }
    }

    [SerializeField] private int clearCoin;
    public int ClearCoin
    {
        get { return clearCoin; }
    }

    [SerializeField] private List<ExpItem_Info> clearExpItem = new List<ExpItem_Info>();
    public List<ExpItem_Info> ClearExpItem
    {
        get { return clearExpItem; }
    }
}
