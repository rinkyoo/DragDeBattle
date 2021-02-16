using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInfo : MonoBehaviour
{
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

    public float GetBorderUpPosi()
    {
        Camera canvasCamera = GetComponentInParent<Canvas>().worldCamera;
        var corners = new Vector3[4];
        GameObject.Find("border").GetComponent<RectTransform>().GetWorldCorners(corners);
        var temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[1]);
        return temp.y;
    }

    public float GetBorderLeftPosi()
    {
        Camera canvasCamera = GetComponentInParent<Canvas>().worldCamera;
        var corners = new Vector3[4];
        GameObject.Find("LeftWallImageBack").GetComponent<RectTransform>().GetWorldCorners(corners);
        var temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[2]);
        return temp.x;
    }

    public float GetBorderRightPosi()
    {
        Camera canvasCamera = GetComponentInParent<Canvas>().worldCamera;
        var corners = new Vector3[4];
        GameObject.Find("RightWallImageBack").GetComponent<RectTransform>().GetWorldCorners(corners);
        var temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[0]);
        return temp.x;
    }
}
