using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//経験値アイテムの情報
[SerializeField]
[CreateAssetMenu(fileName = "ExpItem_Info", menuName = "Create_ExpItem_Info")]
public class ExpItem_Info : ScriptableObject
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
    new private string name;
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
    public int ExpValue
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
