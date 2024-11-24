using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorRange : MonoBehaviour
{
    [SerializeField]private Material m_material;
    [SerializeField][Range(.1f,20.0f)]private float radius;
    [SerializeField][Range(1.0f,360.0f)]private float angle;
    //扇形的顶点数目
    [SerializeField][Range(1,60)]private int quality;
    private MeshFilter m_MeshFilter;//存放网格数据的组件
    private MeshRenderer m_MeshRenderer;
    private GameObject sectorObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //左键更新并显示
        if(Input.GetMouseButtonDown(0)){
            sectorObj = GetSector(Vector3.zero,angle,radius,quality);
            sectorObj.SetActive(true);
        }
        if(Input.GetMouseButtonDown(1)){
            if(sectorObj != null){
                sectorObj.SetActive(false);;
            }
        }
    }

    private GameObject GetSector(Vector3 center,float angle,float radius,int triangleCnt){
        float eachAngle = angle / triangleCnt;
        List<Vector3> verts = new List<Vector3>();
        verts.Add(center);
        for (int i = 0; i < triangleCnt; i++)
        {//(0,1,0)绕z轴逆时针旋转(angle/2 - i)
            Vector3 pos = Quaternion.Euler(0,0,angle / 2 - i * eachAngle) * Vector2.up * radius;//rotation 左乘 position = 旋转后的position
            verts.Add(pos);
        }
        return CreateMesh(verts);
    }

    private GameObject CreateMesh(List<Vector3> vertices){
        int trianglesCnt = vertices.Count - 2;//三角形数目
        int[] triangleVertices = new int[trianglesCnt * 3];
        //三角形顶点排列
        for (int i = 0; i < trianglesCnt; i++)
        {
            triangleVertices[i*3] = 0;
            triangleVertices[i*3+1] = i+1;
            triangleVertices[i*3+2] = i+2;//这里还不是很懂
        }
        //uv
        Vector2[] uvs = new Vector2[vertices.Count];
        uvs[0] = new Vector2(vertices[0].x,vertices[0].y);
        for(int i = 1;i<vertices.Count;i++){
            uvs[i] = new Vector2(vertices[i].x,1);//这表明贴图必须是“一维”的
        }

        if(sectorObj == null){
            sectorObj = new GameObject("Sector");
            sectorObj.transform.SetParent(transform,false);
            m_MeshFilter = sectorObj.AddComponent<MeshFilter>();
            m_MeshRenderer = sectorObj.AddComponent<MeshRenderer>();
        }
        //网格对象
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangleVertices;
        mesh.uv = uvs;
        //渲染
        m_MeshFilter.mesh = mesh;
        m_MeshRenderer.material = m_material;
        return sectorObj;
    }
}
