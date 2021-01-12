using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus //: MonoBehaviour
{
    public int maxHP; public int nowHP;
    public int maxSP; public int nowSP;
    public int str;   public int vit;
    public float agi;
    public float speedAtk; public float rangeAtk;
    
    //ステータスの初期化
    public void SetStatus(Enemy_Info enemy)
    {
        maxHP = nowHP = enemy.HP;
        maxSP = enemy.SP;
        nowSP = 0;
        str = enemy.STR; vit = enemy.VIT;
        agi = enemy.AGI;
        speedAtk = enemy.SPEED_ATK; rangeAtk = enemy.RANGE_ATK;
    }
    
    public void Damage(int damage)
    {
        damage -= vit; //防御力を反映
        if(damage <= 0) damage = 1; //最低でも１ダメージは保証
        nowHP -= damage;
        if(nowHP > maxHP) nowHP = maxHP;
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
