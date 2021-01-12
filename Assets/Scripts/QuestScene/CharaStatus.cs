using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaStatus //: MonoBehaviour
{
    public int maxHP; public int nowHP;
    public int maxSP; public int nowSP;
    public int str;   public int vit;
    public float agi;
    public float speedAtk; public float rangeAtk;
    
    //ステータスの初期化
    public void SetStatus(Chara_Info chara)
    {
        maxHP = nowHP = chara.HP;
        maxSP = chara.SP;
        nowSP = 0;
        str = chara.STR;
        vit = chara.VIT;
        agi = chara.AGI;
        speedAtk = chara.SpeedATK; rangeAtk = chara.RangeATK;
    }
    
    public void Damage(int damage)
    {
        damage -= vit; //防御力を反映
        if(damage <= 0) damage = 1; //最低でも１ダメージは保証
        nowHP -= damage;
        //if(nowHP > maxHP) nowHP = maxHP;
    }
    public void Heal(int heal)
    {
        nowHP += heal;
        if (nowHP > maxHP) nowHP = maxHP;
    }
    
    //SPを回復
    public void GetSP(int i)
    {
        nowSP += i;
        if(nowSP > maxSP) nowSP = maxSP;
    }
    
    public bool isDied()
    {
        return nowHP<=0;
    }
}
