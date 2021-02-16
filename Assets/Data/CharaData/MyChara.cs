using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所持しているキャラの情報
//JSONファイルで管理
//変数宣言をprivateで試したけどうまく値を読み込めず＝＞JSONについて要勉強
[System.Serializable]
public class MyChara
{
    public string name;
    public string Name
    {
        set { name = value; }
        get { return name; }
    }

    public int level;
    public int Level
    {
        set{ level = value; }
        get{ return level; }
    }

    public int nowEXP;
    public int NowEXP
    {
        set { nowEXP = value; }
        get { return nowEXP; }
    }

    public int nextEXP;
    public int NextEXP
    {
        set { nextEXP = value; }
        get { return nextEXP; }
    }

    public int plusNextEXP;
    public int PlusNextEXP
    {
        set { plusNextEXP = value; }
        get { return plusNextEXP; }
    }

    public int hp;
    public int HP
    {
        set{ hp = value; }
        get{ return hp; }
    }
    
    public int sp;
    public int SP
    {
        set{ sp = value; }
        get{ return sp; }
    }
    public int str;
    public int STR
    {
        set{ str = value; }
        get{ return str; }
    }
    
    public int vit;
    public int VIT
    {
        set{ vit = value; }
        get{ return vit; }
    }
    
    public float agi;
    public float AGI
    {
        set{ agi = value; }
        get{ return agi; }
    }
    
    public float speedAtk;
    public float SpeedATK
    {
        set{ speedAtk = value; }
        get{ return speedAtk; }
    }
    
    public float rangeAtk;
    public float RangeATK
    {
        set{ rangeAtk = value; }
        get{ return rangeAtk; }
    }
    
}
