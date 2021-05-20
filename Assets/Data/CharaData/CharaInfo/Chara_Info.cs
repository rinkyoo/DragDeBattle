using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//実装済みキャラの情報
//ステータスはレベル１の値
[Serializable]
[CreateAssetMenu(fileName="Chara_Info",menuName="Create_Chara_Info")]
public class Chara_Info : ScriptableObject
{
    //キャラID
    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
    }
    //キャラ名
    [SerializeField]
    new private string name;
    public string Name
    {
        get { return name; }
    }
    //プレハブ
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab
    {
        get { return prefab; }
    }
    //icon
    [SerializeField]
    private Sprite icon;
    public Sprite Icon
    {
        get { return icon; }
    }
    //レベル
    [SerializeField]
    private int level;
    public int Level
    {
        set { level = value; }
        get { return level; }
    }
    //レベルアップに必要な経験値
    [SerializeField]
    private int nextEXP;
    public int NextEXP
    {
        set { nextEXP = value; }
        get { return nextEXP; }
    }
    //nextEXPの加算値
    [SerializeField]
    private int plusNextEXP;
    public int PlusNextEXP
    {
        set { plusNextEXP = value; }
        get { return plusNextEXP; }
    }
    //現在の所持経験値
    [SerializeField]
    private int nowEXP;
    public int NowEXP
    {
        set { nowEXP = value; }
        get { return nowEXP; }
    }
    //HP
    [SerializeField]
    private int hp;
    public int HP{
        set { hp = value; }
        get { return hp;}
    }
    //レベルアップ時のHP増加値
    [SerializeField]
    private int addHp;
    public int AddHP
    {
        set { addHp = value; }
        get { return addHp; }
    }
    //スキル発動に必要なSP
    [SerializeField]
    private int sp;
    public int SP{
        get{ return sp;}
    }
    //スキル情報
    [SerializeField]
    private string skillInfo;
    public string SkillInfo
    {
        get { return skillInfo; }
    }
    //攻撃力
    [SerializeField]
    private int str;
    public int STR{
        set { str = value; }
        get { return str;}
    }
    //レベルアップ時のSTR増加値
    [SerializeField]
    private int addStr;
    public int AddSTR
    {
        set { addStr = value; }
        get { return addStr; }
    }
    //防御力
    [SerializeField]
    private int vit;
    public int VIT{
        set { vit = value; }
        get { return vit;}
    }
    //レベルアップ時のVIT増加値
    [SerializeField]
    private int addVit;
    public int AddVIT{
        set { addVit = value; }
        get { return addVit;}
    }
    //移動速度
    [SerializeField]
    private float agi;
    public float AGI{
        set { agi = value; }
        get { return agi;}
    }
    //攻撃間隔
    [SerializeField]
    private float speed_atk;
    public float SpeedATK{
        set { speed_atk = value; }
        get { return speed_atk;}
    }
    //攻撃範囲
    [SerializeField]
    private float range_atk;
    public float RangeATK{
        set { range_atk = value; }
        get { return range_atk;}
    }
    //近距離OR遠距離
    [SerializeField]
    private string attackType;
    public string AttackType
    {
        get { return attackType; }
    }
    //キャラ特性の説明
    [SerializeField]
    private string charaInfoText;
    public string CharaInfoText
    {
        get { return charaInfoText; }
    }
    //レベルの上限値
    [SerializeField]
    private int maxLevel;
    public int MaxLevel
    {
        get { return maxLevel; }
    }
    //レベル１のときのnextEXP
    [SerializeField]
    private int level1NextEXP;
    public int Level1NextEXP
    {
        get { return level1NextEXP; }
    }
    //レベル１のときのplusNextEXP
    [SerializeField]
    private int level1PlusNextEXP;
    public int Level1PlusNextEXP
    {
        get { return level1PlusNextEXP; }
    }
    //レベル１のときのhp
    [SerializeField]
    private int level1HP;
    public int Level1HP
    {
        get { return level1HP; }
    }
    //レベル１のときのstr
    [SerializeField]
    private int level1STR;
    public int Level1STR
    {
        get { return level1STR; }
    }
    //レベル１のときのvit
    [SerializeField]
    private int level1VIT;
    public int Level1VIT
    {
        get { return level1VIT; }
    }

    public Chara_Info Clone()
    {
        return (Chara_Info)MemberwiseClone();
    }
}
