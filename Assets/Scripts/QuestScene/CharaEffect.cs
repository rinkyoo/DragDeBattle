using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaEffect : MonoBehaviour
{
    public GameObject PCAppearParticle;
    public GameObject EnemyAppearParticle;
    
    public void SetPCAppearParticle(Vector3 posi)
    {
        Instantiate(PCAppearParticle,posi,Quaternion.Euler(-90f,0,0));
    }
    
    public void SetEnemyAppearParticle(Vector3 posi)
    {
        Instantiate(EnemyAppearParticle,posi,Quaternion.Euler(-90f,0,0));
    }


}
