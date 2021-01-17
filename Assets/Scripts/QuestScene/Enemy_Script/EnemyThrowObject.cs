using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrowObject : MonoBehaviour
{
    Vector3 offset;
    Vector3 target; //ë_Ç§ìGÇÃç¿ïW

    float m;
    float x,y,z = 0;
    float xD;
    float a,b;
    float yzero_x;
    float time = 0;
    bool isThrowed = false;
    
    public void SetThrow(Vector3 posi)
    {
        offset = transform.position;
        target = posi - offset;
        xD = target.x;
        m = target.z / target.x;
        isThrowed = true;
        
        a = -0.2f;
        b = 3.7f;
        yzero_x = b / a * -1f;
        
        //b = Mathf.Tan (deg * Mathf.Deg2Rad);
        //a = (target.y - b * target.x) / (target.x * target.x);
    }
    
    void FixedUpdate()
    {
        if(!isThrowed) return;
        time += Time.deltaTime;
        x = time * xD;
        float xsub = time * yzero_x;
        y = a * xsub * xsub + b * xsub;
        z = m * x;
        transform.position = new Vector3 (x, y, z) + offset;
        if(transform.position.y < -30) Destroy(this.gameObject);
    }
    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Field"))
        {
            Destroy(this.gameObject);
            return;
        }
    }
}
