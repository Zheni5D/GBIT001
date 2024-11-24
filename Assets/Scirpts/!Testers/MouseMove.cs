using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    public float speed;
    public Vector3 size;
    public Vector3 lookAtSize;
    public float lookAtRadius;
    public GameObject target;
    [Tooltip("为什么会有两个target呢?target是准心,lookAtTarget被镜头跟随")]
    public GameObject lookAtTarget;
    private PlayerControl playerControl;
    bool isPressLShift;
    Vector3 curPlayerPos;
    Vector3 lastPlayerPos;
    void Awake()
    {
        MessageCenter.AddListener(OnGameOver);
    }
    private void Start() {
        target.transform.position = transform.position;
        lookAtTarget.transform.position = transform.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if(TryGetComponent<PlayerControl>(out playerControl))
        {
            curPlayerPos = playerControl.transform.position;
            lastPlayerPos = curPlayerPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region 按键功能

        // if(Input.GetKeyDown(KeyCode.R))//调试用
        // {
        //     target.transform.position = Vector3.zero;
        // }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            size *= 2;
            isPressLShift=true;
            Vector3 correctOffset = getZoomInOutOffset(transform.position,target.transform.position,true);
            target.transform.position += correctOffset;
            //lookAtTarget.transform.position += correctOffset;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            size /= 2;
            isPressLShift=false;
            Vector3 correctOffset = getZoomInOutOffset(transform.position,target.transform.position,false);
            target.transform.position += correctOffset;
            //lookAtTarget.transform.position += correctOffset;
        }
        #endregion

        #region 移动控制

        #region target跟随玩家物体移动

        if(playerControl != null)
        {
            curPlayerPos = playerControl.transform.position;
            Vector3 dirOffset = curPlayerPos - lastPlayerPos;
            target.transform.position += dirOffset;
            // lookAtTarget.transform.position += dirOffset;
            lastPlayerPos = curPlayerPos;
        }

        #endregion

        float mouseX = Input.GetAxis ("Mouse X") * speed;
        float mouseY = Input.GetAxis ("Mouse Y") * speed;

        #region 限制target在大小为size的圆or矩形内
        Vector3 nextPos = target.transform.position + new Vector3(mouseX,mouseY);
        Vector3 finalPos = Vector3.zero;
        bool isVerticalInput = !Mathf.Approximately(mouseX,0);
        bool isHorizongtalInput = !Mathf.Approximately(mouseY,0);
        if(isVerticalInput || isHorizongtalInput)
        {
            float edgeL = transform.position.x - size.x * .5f;
            float edgeR = transform.position.x + size.x * .5f;
            float edgeU = transform.position.y + size.y * .5f;
            float edgeD = transform.position.y - size.y * .5f;
           
            if(nextPos.x < edgeR && nextPos.x > edgeL)
            {
                finalPos.x = mouseX;
            }
            if(nextPos.y < edgeU && nextPos.y > edgeD)
            {
                finalPos.y = mouseY;
            }
            target.transform.position += finalPos;
        }
        #endregion
        
        #region lookAtTarget部分--限制在某个矩形内
        if(lookAtTarget != null)
        {
            if(isPressLShift)
            {
                float edgeL = transform.position.x - lookAtSize.x * .5f;
                float edgeR = transform.position.x + lookAtSize.x * .5f;
                float edgeU = transform.position.y + lookAtSize.y * .5f;
                float edgeD = transform.position.y - lookAtSize.y * .5f;
                Vector3 targetPos = target.transform.position;
                if(targetPos.x > edgeR)//水平方向在范围外
                    targetPos.x = edgeR;
                else if(targetPos.x < edgeL)
                    targetPos.x = edgeL;
                if(targetPos.y > edgeU)//垂直方向在范围外
                    targetPos.y = edgeU;
                else if(targetPos.y < edgeD)
                    targetPos.y = edgeD;
                lookAtTarget.transform.position = targetPos;
            }else{
                Vector3 dir = target.transform.position - transform.position;
                if(dir.magnitude > lookAtRadius)
                {
                    finalPos = dir.normalized * lookAtRadius + transform.position;
                    lookAtTarget.transform.position = finalPos;
                }
                else
                {
                    lookAtTarget.transform.position = target.transform.position;
                }
            }    
        }
        
        #endregion
#endregion

    }
    void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameOver);
    }
    // private void OnDrawGizmosSelected() {
    //     Gizmos.DrawWireCube(transform.position,size);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position,lookAtSize);
    //     // float edgeL = transform.position.x - size.x / 2;
    //     // float edgeR = transform.position.x + size.x / 2;
    //     // float edgeU = transform.position.y + size.y / 2;
    //     // float edgeD = transform.position.y - size.y / 2;
    //     // Gizmos.color = Color.cyan;
    //     // Gizmos.DrawWireSphere(new Vector3(edgeL,transform.position.y),1f);
    //     // Gizmos.DrawWireSphere(new Vector3(transform.position.x,edgeD),1f);
    // }

    Vector3 getZoomInOutOffset(Vector3 ori,Vector3 tar,bool isZoomIn)
    {
        Vector3 dir = tar - ori;
        if(isZoomIn)
        {
            return dir;
        }
        else
        {
            return -dir / 2;
        }
    }

    void OnGameOver(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_OVER) return;
        isPressLShift = false;
        target.transform.position = transform.position;
    }

}
