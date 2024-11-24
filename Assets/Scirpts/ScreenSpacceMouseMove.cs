using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSpacceMouseMove : MonoBehaviour
{
    public RectTransform target;
    public RectTransform canvasRect;
    public Vector3 size;
    public float speed = 0.1f;
    public float lookAtRadius;
    [Tooltip("为什么会有两个target呢?因为我们要的效果是镜头会跟随准心,准心不会离开某个矩形，而镜头最多不离开某个圆")]
    public GameObject lookAtTarget;//在世界坐标中计算
    //public Vector2 pivotPos_InScreenSpace;
    public Vector3 localPos;
    public Vector3 worldPos;
    public Vector3 funcPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        localPos = target.localPosition;
        worldPos = target.position;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(target,Input.mousePosition,null,out funcPos);

        float mouseX = Input.GetAxis ("Mouse X") * speed;
        float mouseY = Input.GetAxis ("Mouse Y") * speed;

        #region 限制target在大小为size的矩形内
        //(假设:target的父物体没有进行旋转和缩放,初始状态下target与父物体相对位置为(0,0,0))
        //由于是在屏幕空间所以不用实现跟随玩家的逻辑，target和玩家的相对位置是固定的，除非动鼠标
        //因此玩家的移动不会影响target和玩家的相对位置
        Vector3 nextPos = target.localPosition + new Vector3(mouseX,mouseY);
        Vector3 finalPos = Vector3.zero;
        bool isVerticalInput = !Mathf.Approximately(mouseX,0);
        bool isHorizongtalInput = !Mathf.Approximately(mouseY,0);
        //pivotPos_InScreenSpace = RectTransformUtility.WorldToScreenPoint(Camera.main,pivot.transform.position);
        if(isVerticalInput || isHorizongtalInput)
        {
            float edgeL = - size.x / 2;
            float edgeR = size.x / 2;
            float edgeU = size.y / 2;
            float edgeD = - size.y / 2;
           
            if(nextPos.x < edgeR && nextPos.x > edgeL)
            {
                finalPos.x = mouseX;
            }
            if(nextPos.y < edgeU && nextPos.y > edgeD)
            {
                finalPos.y = mouseY;
            }
            target.localPosition += finalPos;
        }
        #endregion

        #region lookAtTarget部分--限制在某个圆内
        if(lookAtTarget != null)
        {
            // Vector3 dir = target.transform.position;
            // if(dir.magnitude > lookAtRadius)
            // {
            //     finalPos = dir.normalized * lookAtRadius + transform.position;
            //     lookAtTarget.transform.position = finalPos;
            // }
            // else
            // {
            //     lookAtTarget.transform.position = target.transform.position;
            // }
            lookAtTarget.transform.position += new Vector3(mouseX,mouseY) / 10;
        }
        
        #endregion
    }
}
