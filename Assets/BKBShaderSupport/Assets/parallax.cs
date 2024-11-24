using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    public float speed;
    public bool isRepeated;
    Vector3 origPos;
    Transform curTrans;
    float texWidth;//纹理在世界坐标下的宽度
    // Start is called before the first frame update
    void Start()
    {
        curTrans = Camera.main.transform;
        origPos = curTrans.position; 
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture texture = sprite.texture;
        texWidth = texture.width / sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() {
        //计算
        Vector3 offset = curTrans.position - origPos;
        //更新
        origPos = curTrans.position;
        transform.position += new Vector3(offset.x * speed,0,0);

        //跃迁
        if(isRepeated)
        {
            float distance = Mathf.Abs(transform.position.x - curTrans.position.x);
            if(distance >= texWidth)
            {
                float Xoffset = distance % texWidth;
                transform.position = new Vector3(curTrans.position.x + Xoffset,transform.position.y,transform.position.z);
                //                                  tmd这里是摄像机的位置上加偏移量，而不是贴图原来的位置加偏移量
            }
        }
        
    }

}
