using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class RoutineShadow : MonoBehaviour
{
    public float max_record_time = 5f;
    public float record_interval = .2f;
    private List<SpriteRenderer> shadows;
    private SpriteRenderer spr;
    [SerializeField]private bool isRecording = false;
    private int shadowsCount;
    private int readyShadowCount;
    private IEnumerator runningItor;
    private Coroutine runningCorotine;
    [SerializeField]private Color shadowColor;
    [SerializeField]private Material windMaterial;
    [SerializeField]private GameObject windedPaticleGO;
    private LineRenderer lineRenderer;
    private List<Vector3> lineRendererPoint;
    private bool isGamePause = false;
    public static class MiniPool{//自作聪明
        private static GameObject windedPaticleGO;
        private static GameObject pool;
        public static GameObject Pool{
            get{
                if(pool == null){
                    pool = new GameObject("ObejctPool");
                    DontDestroyOnLoad(pool);
                }
                return pool;
            }
        }
        private static Queue<GameObject> windedParticlePool;
        public static GameObject GetWindedParticle(){
            if(windedParticlePool == null){
                windedParticlePool = new Queue<GameObject>();
            }
            GameObject g;
            if(windedParticlePool.Count != 0){
                g = windedParticlePool.Dequeue();
                g.SetActive(true);
                return g;
            }
            g = Instantiate(windedPaticleGO);
            g.transform.SetParent(Pool.transform);
            return g;
        }
        public static void DeleteWindedParticle(GameObject g){
            if(windedParticlePool == null){
                windedParticlePool = new Queue<GameObject>();
            }
            g.SetActive(false);
            windedParticlePool.Enqueue(g);
        }
        public static void ClearPool(){
            if(windedParticlePool==null) return;
            while(windedParticlePool.Count > 0){
                Destroy(windedParticlePool.Dequeue());
            }
        }
        public static void SetWindedParticleGO(GameObject g){
            windedPaticleGO = g;
        }
    }
    private void OnApplicationQuit() {
        MiniPool.ClearPool();
    }
    
    
    private void OnEnable() {
        MessageCenter.AddListener(OnTimeStopOff);
        MessageCenter.AddListener(OnTimeStopOn);
        MessageCenter.RemoveListner(OnGamePauseOn);
        MessageCenter.AddListener(OnGamePauseOff);
        windMaterial.SetFloat("_speed",3f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        shadows = new List<SpriteRenderer>();
        lineRendererPoint = new List<Vector3>();
        spr = GetComponent<SpriteRenderer>();
        MiniPool.SetWindedParticleGO(windedPaticleGO);
        shadowsCount =Mathf.RoundToInt(max_record_time / record_interval);
        GameObject parent = new GameObject("RoutineParent");
        for (int i = 0; i < shadowsCount; i++)
        {
            SpriteRenderer s = new GameObject("Shadow"+i).AddComponent<SpriteRenderer>();
            s.transform.SetParent(parent.transform);
            s.sortingLayerName = spr.sortingLayerName;
            s.sortingOrder = spr.sortingOrder;
            s.sprite = spr.sprite;
            s.color = new Color(0,0,0,.5f);
            s.gameObject.SetActive(false);
            shadows.Add(s);
        }
        GameObject windLine = new GameObject("WindLine");
        windLine.transform.SetParent(parent.transform);
        lineRenderer = windLine.AddComponent<LineRenderer>();
        lineRenderer.sortingLayerName = spr.sortingLayerName;
        lineRenderer.sortingOrder = spr.sortingOrder;
        lineRenderer.receiveShadows = false;
        lineRenderer.material = windMaterial;
        lineRenderer.gameObject.SetActive(false);
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnTimeStopOff);
        MessageCenter.RemoveListner(OnTimeStopOn);
        MessageCenter.RemoveListner(OnGamePauseOn);
        MessageCenter.RemoveListner(OnGamePauseOff);
        stopGenerateShadows();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha9)){
            if(!isRecording){
                playGenerateShadows();
            }
        }
    }

    

    IEnumerator generateShadows(){
        isRecording = true;
        readyShadowCount = 0;
        lineRendererPoint.Clear();
        for(int i = 0;i<shadowsCount;i++){
            while (isGamePause)
            {
                //死循环
            }
            //info info = new info(transform.position,spr.sprite);
            shadows[i].gameObject.SetActive(false);
            shadows[i].sprite = spr.sprite;
            shadows[i].transform.position = transform.position;
            shadows[i].color = shadowColor;
            shadows[i].transform.rotation = transform.rotation;
            ++readyShadowCount;
            lineRendererPoint.Add(transform.position);
            yield return new WaitForSeconds(record_interval);
        }
        ShowRoutine();
        isRecording = false;
    }

    private void ShowRoutine(){
        //shadow
        int cnt = 1;
        for(int i = 0;i<readyShadowCount;i++)
        {
            shadows[i].gameObject.SetActive(true);
            shadows[i].DOFade(0,cnt * 0.03f).SetEase(Ease.InExpo);
            cnt++;
        }
        //wind
        lineRendererPoint.Add(transform.position);
        lineRenderer.positionCount = lineRendererPoint.Count;
        int index;
        for(index = 0;index<lineRendererPoint.Count-1;index++){
            Vector3 sourcePos = lineRendererPoint[index];
            if(index % 5 == 0){
                Vector3 targetPos = lineRendererPoint[index+1];
                Vector3 dir = targetPos - sourcePos;
                dir.z = 0;
                GameObject g = MiniPool.GetWindedParticle();
                g.transform.position = targetPos;
                g.transform.rotation = Quaternion.identity;
                float angle =Vector3.SignedAngle(Vector3.up,dir,Vector3.forward);
                Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
                g.transform.rotation = quaternion * g.transform.rotation;
                ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                m.stopAction = ParticleSystemStopAction.Callback;
            }
            lineRenderer.SetPosition(index,sourcePos);
        }
        lineRenderer.SetPosition(index,lineRendererPoint[index]);
        lineRenderer.gameObject.SetActive(true);
        windMaterial.DOFloat(0f,"_speed",2.0f).SetEase(Ease.Linear).OnComplete(()=>{
            lineRenderer.gameObject.SetActive(false);
            windMaterial.SetFloat("_speed",3f);
        }).SetAutoKill(true);
    }

    private void playGenerateShadows(){
        if(isRecording == false){
            runningItor = generateShadows();
            runningCorotine = StartCoroutine(runningItor);
        }
    }

    private void stopGenerateShadows(){
        if(isRecording){
            if(runningCorotine != null){
                StopCoroutine(runningCorotine);
                runningCorotine = null;
            }
            ShowRoutine();
            isRecording = false;
        }
    }

    private void deleteWindedParticle(GameObject go){
        MiniPool.DeleteWindedParticle(go);
    }

    public void OnTimeStopOff(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_OFF) return;
        stopGenerateShadows();   
    }

    public void OnTimeStopOn(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_ON) return;
        playGenerateShadows();   
    }

    public void OnGamePauseOn(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_ON) return;
        isGamePause = true;
    }
    public void OnGamePauseOff(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_OFF) return;
        isGamePause = false;
    }

}
