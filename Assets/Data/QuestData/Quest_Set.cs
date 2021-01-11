using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Quest_Set",menuName="Create_Quest_Set")]
public class Quest_Set : ScriptableObject
{
    [SerializeField]
    private List<Quest_Enemy> quests = new List<Quest_Enemy>();
    
    public List<Quest_Enemy> GetQuestSet()
    {
        return quests;
    }
    public Quest_Enemy GetQuests(int i)
    {
        return quests[i];
    }
}
