using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldEffect : MonoBehaviour
{
    LineRenderer lineRen;
    public GameObject enemyLock;
    
    void Awake()
    {
        lineRen = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
    }
    
    public void SetLineRen(Vector3 posi)
    {
        lineRen.gameObject.SetActive(true);
        lineRen.SetPosition(0,posi);
        lineRen.SetPosition(1,posi);
    }
    public void UpdateLineRen(Vector3 posi)
    {
        lineRen.SetPosition(1,posi);
    }
    
    public void UpdateEnemyLock(Vector3 posi)
    {
        enemyLock.transform.position = posi;
        enemyLock.gameObject.SetActive(true);
    }
    public void HideEnemyLock()
    {
        enemyLock.gameObject.SetActive(false);
    }
    
    public void DeleteDragEffect()
    {
        lineRen.gameObject.SetActive(false);
        enemyLock.transform.position = new Vector3(-30,0,0);
        enemyLock.gameObject.SetActive(false);
    }
}
