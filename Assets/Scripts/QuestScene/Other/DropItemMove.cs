using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropItemMove : MonoBehaviour
{
    //アイテムが回収される座標
    Vector3 movePosi = new Vector3(20f, 3f, -20f);
    float moveSpeed = 0.05f;
    Sequence seq;

    void Start()
    {

    }
    
    void OnEnable()
    {
        Vector3 boundPosi = new Vector3(Random.Range(-7f,7f), -8f, Random.Range(-7f, 7f));
        float distance = Vector3.Distance(transform.position, movePosi);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0f)
            .Append(transform.DOMove(boundPosi,1f).SetRelative().SetEase(Ease.OutBounce))
            .AppendInterval(1f)
            .Append(transform.DOMove(movePosi,distance * moveSpeed))
            .AppendCallback(() =>
            {
                gameObject.SetActive(false);
            });
        seq.Play();
    }
    
}
