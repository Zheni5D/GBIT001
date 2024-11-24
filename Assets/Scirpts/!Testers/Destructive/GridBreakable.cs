using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridBreakable : Breakable
{
    class TileInfo{
        public int i;
        public int j;
        public TileBase tileBase;
        public Matrix4x4 transform;
        public TileInfo(int _i,int _j,TileBase tileBase,Matrix4x4 transform){
            i = _i;
            j = _j;
            this.tileBase = tileBase;
            this.transform = transform;
        }
        public TileInfo(Vector3Int vector,TileBase tileBase,Matrix4x4 transform){
            i = vector.x;
            j = vector.y;
            this.tileBase = tileBase;
            this.transform = transform;
        }
    }
    private Tilemap tilemap;
    [SerializeField]private Tilemap tilemapWithoutBreakable;
    [Header("用于替换破碎的玻璃TileBase")]
    [TextArea]
    public string INFORMATION;
    [SerializeField]private TileBaseDictionarySO tileBaseDictionarySO;
    [SerializeField]private GameObject BrokenGlass;
    private List<TileInfo> tileList = new List<TileInfo>();
    protected override void Start()
    {
        base.Start();
        tilemap = GetComponent<Tilemap>();
    }

    public override void GetHurt(Vector3 murderPoint)
    {
        GetHurt(murderPoint,murderPoint);
    }

    public override void GetHurt(Vector3 murderPoint, Vector3 collidePoint)
    {
        var cellPos = tilemap.WorldToCell(collidePoint);
        // Debug.Log(collidePoint);
        // Debug.Log(cellPos);
        var targettile = tilemap.GetTile(cellPos);
        
        if(targettile ==null){//如果为空则上下左右遍历一次
            var newCellPos = cellPos + Vector3Int.up;
            if(tilemap.GetTile(newCellPos) == null){
                newCellPos = cellPos + Vector3Int.down;
                if(tilemap.GetTile(newCellPos) == null){
                    newCellPos = cellPos + Vector3Int.left;
                    if(tilemap.GetTile(newCellPos) == null){
                        newCellPos = cellPos + Vector3Int.right;
                        if(tilemap.GetTile(newCellPos) == null){
                            return;
                        }else{
                            cellPos = newCellPos;
                        }
                    }else{
                        cellPos = newCellPos;
                    }
                }else{
                    cellPos = newCellPos;
                }
            }else{
                cellPos = newCellPos;
            }
        }
        var tile = tilemap.GetTile(cellPos);        
        TileInfo info = new TileInfo(cellPos,tile,tilemap.GetTransformMatrix(cellPos));
        tileList.Add(info);
        tilemapWithoutBreakable.SetTile(cellPos,tileBaseDictionarySO.GetValue(tile));
        tilemapWithoutBreakable.SetTransformMatrix(cellPos,tilemap.GetTransformMatrix(cellPos));
        tilemapWithoutBreakable.RefreshTile(cellPos);
        tilemap.SetTile(cellPos,null);
        tilemap.RefreshTile(cellPos);        
        GameObject g = Instantiate(BrokenGlass,collidePoint,Quaternion.identity);
        g.GetComponent<Split>().TriggerInit();
        //计算破碎方向
        //true=左右false=上下,45度归左右
        Vector3 dir = collidePoint - murderPoint;
        dir.z=0;
        float angle = Vector3.Angle(Vector3.right,dir);//结果永远不会超过 180 度
        //0-45,135-180是左右
        if(angle >=0 && angle <= 45f || angle >= 135f && angle <= 180f)
        {
            g.GetComponent<Split>().Trigger(murderPoint);
        }
        else
        {
            g.GetComponent<Split>().Trigger(murderPoint,false);
        } 
        SoundManager.PlayAudioWithLimit("glassBroken");
        HearingPoster.PostVoice(collidePoint, soundRadius);
    }

    public override void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        int stageId = msg.intParam;//StageID start with 0
        foreach(TileInfo info in tileList){
            Vector3Int cellPos = new Vector3Int(info.i,info.j,0);
            tilemap.SetTile(cellPos,info.tileBase);
            tilemap.SetTransformMatrix(cellPos,info.transform);
            tilemap.RefreshTile(cellPos);
            tilemapWithoutBreakable.SetTile(cellPos,null);
            tilemapWithoutBreakable.RefreshTile(cellPos);
        }
    }

    public void OnMovNextStage(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
        tileList.Clear();
    }

    protected override void Awake()
    {
        base.Awake();
        MessageCenter.AddListener(OnMovNextStage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        MessageCenter.RemoveListner(OnMovNextStage);
    }
}
