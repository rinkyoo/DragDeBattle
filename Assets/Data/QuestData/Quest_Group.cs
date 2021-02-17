using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "Quest_Group", menuName= "Create_Quest_Group")]
public class Quest_Group : ScriptableObject
{
    [SerializeField]
    private List<Quest_Data> quests = new List<Quest_Data>();

    [SerializeField] private string setName;
    public string SetName
    {
        get { return setName; }
    }

    public List<Quest_Data> GetQuestList()
    {
        return quests;
    }
    public Quest_Data GetQuests(int i)
    {
        return quests[i];
    }
}
