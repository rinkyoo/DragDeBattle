using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//経験値アイテムの情報
[SerializeField]
[CreateAssetMenu(fileName = "EXPItem_Info", menuName = "Create_EXPItem_Info")]
public class EXPItem_Info : ScriptableObject
{
    //アイテムID
    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
    }
    //アイテム名
    [SerializeField]
    private string name;
    public string Name
    {
        get { return name; }
    }
    //icon
    [SerializeField]
    private Sprite icon;
    public Sprite Icon
    {
        get { return icon; }
    }
    /*
    //所持個数
    [SerializeField]
    private int num;
    public int Num
    {
        set { num = value; }
        get { return num; }
    }
    */
    //経験値量
    [SerializeField]
    private int expValue;
    public int EXPValue
    {
        get { return expValue; }
    }
    //必要コイン量
    [SerializeField]
    private int coin;
    public int Coin
    {
        get { return coin; }
    }
}
