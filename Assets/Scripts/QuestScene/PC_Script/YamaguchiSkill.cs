using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YamaguchiSkill : MonoBehaviour
{
    private QuestController qc; //スキル中に画面クリックを無効にするために使用（もっと賢い方法がいいかも）

    private List<GameObject> hitList = new List<GameObject>(); //スキルが当たったキャラリスト
    [SerializeField] private GameObject damageEffect;
    private Vector3 charaPosi;
    private int str;
    private float skillTime = 5f; //スキル発動時間
    private float elapsedTime;
    private float angle;
    private float radius;
    private bool flag = false;
    
    int LayerMask = 1 << 11; //Enemy用のレイヤー
    
    void Start()
    {
        charaPosi = transform.position;
        elapsedTime = 0;
        angle = 0;
        radius = 0;
    }
    
    public void SetSkill(int setSTR)
    {
        qc = GameObject.Find("QuestController").GetComponent<QuestController>();
        qc.clickBlock.gameObject.SetActive(true);
        this.str = setSTR;
        flag = true;
    }

    void Update()
    {
        if(flag)
        {
            float x = charaPosi.x + ( Mathf.Cos(angle) * radius );
            float z = charaPosi.z + ( Mathf.Sin(angle) * radius );
            transform.position = new Vector3(x,3f,z);
            
            Collider[] colliders = Physics.OverlapSphere(transform.position,0.7f,LayerMask);
            foreach (Collider hit in colliders)
            {
                if(hit.gameObject.CompareTag("Enemy"))
                {
                    //接触判定が重複しないように判定式を入れる
                    if( !hitList.Contains(hit.gameObject) )
                    {
                        hitList.Add(hit.gameObject);
                        GameObject effect = Instantiate(damageEffect) as GameObject;
                        effect.transform.position = hit.gameObject.transform.position;//ClosestPointOnBounds(this.transform.position);
                    }
                }
            }
            
            elapsedTime += Time.unscaledDeltaTime;
            //スキル終了時
            if(elapsedTime >= skillTime)
            {
                qc.ResumeBattle();
                qc.clickBlock.gameObject.SetActive(false);
                foreach (GameObject hit in hitList)
                {
                    //スキルのダメージ反映 かつ スキルで敵キャラが倒れたかの判定
                    if( hit.GetComponent<EnemyController>().Attacked(str * 3) )
                    {
                        //スキルで倒した時の演出などあればここに記述（現状ない）
                    }
                }
                Destroy(this.gameObject);
            }
            
            angle += 0.05f;
            radius += 0.05f;
        }
    }
    
}
