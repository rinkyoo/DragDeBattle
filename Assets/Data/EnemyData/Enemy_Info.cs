using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
[CreateAssetMenu(fileName="Enemy_Info",menuName="Create_Enemy_Info")]
public class Enemy_Info : ScriptableObject
{
    //キャラ名
    [SerializeField]
    public string chara_name;
    /*
    //プレハブ名
    [SerializeField]
    public string prefab_name;
    */
    //プレハブ
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab
    {
        get { return prefab; }
    }
    //レベル
    [SerializeField]
    public int level;
    //HP
    [SerializeField]
    private int hp;
    public int HP{
        get{ return hp;}
    }
    //スキル発動に必要なSP
    [SerializeField]
    private int sp;
    public int SP{
        get{ return sp;}
    }
    //攻撃力
    [SerializeField]
    private int str;
    public int STR{
        get{ return str;}
    }
    //防御力
    [SerializeField]
    private int vit;
    public int VIT{
        get{ return vit;}
    }
    //移動速度
    [SerializeField]
    private float agi;
    public float AGI{
        get{ return agi;}
    }
    //攻撃速度
    [SerializeField]
    private float speed_atk;
    public float SPEED_ATK{
        get{ return speed_atk;}
    }
    //攻撃範囲
    [SerializeField]
    private float range_atk;
    public float RANGE_ATK{
        get{ return range_atk;}
    }
    //ドロップするコイン
    [SerializeField]
    private int dropCoin;
    public int DropCoin
    {
        get { return dropCoin; }
    }
    //ドロップする経験値アイテム
    [SerializeField]
    private EXPItem_Info dropExpItem;
    public EXPItem_Info DropEXPItem
    {
        get { return dropExpItem; }
    }
    //経験値アイテムのドロップ確率
    [SerializeField]
    private int expItem_Kakuritsu;
    public int EXPItemKakuritsu
    {
        get { return expItem_Kakuritsu; }
    }
}
