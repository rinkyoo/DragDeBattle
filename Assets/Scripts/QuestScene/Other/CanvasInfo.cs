using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInfo : MonoBehaviour
{
    [SerializeField] GameObject wallDown;
    [SerializeField] GameObject wallLeft;
    [SerializeField] GameObject wallRight;

    Camera canvasCamera;

    float borderDownPosi;
    float borderLeftPosi;
    float borderRightPosi;

    void Awake()
    {
        canvasCamera = GetComponentInParent<Canvas>().worldCamera;

        #region フィールドとUIの境目の座標を取得
        var corners = new Vector3[4];
        GameObject.Find("border").GetComponent<RectTransform>().GetWorldCorners(corners);
        var temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[1]);
        borderDownPosi = temp.y;
        GameObject.Find("LeftWallImageBack").GetComponent<RectTransform>().GetWorldCorners(corners);
        temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[2]);
        borderLeftPosi = temp.x;
        GameObject.Find("RightWallImageBack").GetComponent<RectTransform>().GetWorldCorners(corners);
        temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[0]);
        borderRightPosi = temp.x;
        #endregion

        #region フィールド上の壁を境目の座標に移動
        int layerMask = 1 << 9;
        Camera mainCamera = GameObject.Find("QuestMainCamera").GetComponent<Camera>();
        //DownWall
        Vector3 posi = new Vector3(50f, borderDownPosi, 0);
        Vector3 worldPosi = mainCamera.ScreenToWorldPoint(posi);
        Ray ray = new Ray(worldPosi, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 300f, Color.red, 30f, false);
        foreach (RaycastHit hit in Physics.RaycastAll(ray, layerMask))
        {
            if (hit.transform.gameObject.CompareTag("Field"))
            {
                float z = hit.point.z - (wallDown.transform.localScale.z / 1.7f);
                Vector3 setPosi = wallDown.transform.position;
                setPosi.z = z;
                wallDown.transform.position = setPosi;
            }
        }
        //LeftWall
        posi = new Vector3(borderLeftPosi, 100f, 0);
        worldPosi = mainCamera.ScreenToWorldPoint(posi);
        ray = new Ray(worldPosi, mainCamera.transform.forward);
        foreach (RaycastHit hit in Physics.RaycastAll(ray, layerMask))
        {
            if (hit.transform.gameObject.CompareTag("Field"))
            {
                float x = hit.point.x - (wallLeft.transform.localScale.x / 1.7f);
                Vector3 setPosi = wallLeft.transform.position;
                setPosi.x = x;
                wallLeft.transform.position = setPosi;
                print(hit.point + " : " + setPosi);

            }
        }
        //RightWall
        posi = new Vector3(borderRightPosi, 100f, 0);
        worldPosi = mainCamera.ScreenToWorldPoint(posi);
        ray = new Ray(worldPosi, mainCamera.transform.forward);
        foreach (RaycastHit hit in Physics.RaycastAll(ray, layerMask))
        {
            if (hit.transform.gameObject.CompareTag("Field"))
            {
                float x = hit.point.x + (wallRight.transform.localScale.x / 1.7f);
                Vector3 setPosi = wallRight.transform.position;
                setPosi.x = x;
                wallRight.transform.position = setPosi;
            }
        }
        #endregion
    }

    public Vector3 GetCanvasPosi()
    {
        var rect = this.GetComponent<RectTransform>();
        Vector3 foge = new Vector3();
        foge.x = rect.position.x - (rect.sizeDelta.x/2f * rect.localScale.x);
        foge.y = rect.position.y - (rect.sizeDelta.y/2f * rect.localScale.y);
        foge.z = rect.position.z - 5f;
        
        return foge;
    }
    public float GetCanvasScale()
    {
        var rect = this.GetComponent<RectTransform>();
        return rect.localScale.x;
    }

    public float GetBorderDownPosi()
    {
        return borderDownPosi;
    }
    public float GetBorderLeftPosi()
    {
        return borderLeftPosi;
    }
    public float GetBorderRightPosi()
    {
        return borderRightPosi;
    }
}
