using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "Quest_Group", menuName= "Create_Quest_Group")]
public class Quest_Group : ScriptableObject
{
    [SerializeField]
    private List<Quest_Enemy> quests = new List<Quest_Enemy>();

    [SerializeField] private string setName;
    public string SetName
    {
        get { return setName; }
    }

    public List<Quest_Enemy> GetQuestList()
    {
        return quests;
    }
    public Quest_Enemy GetQuests(int i)
    {
        return quests[i];
    }
}
