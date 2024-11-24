using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class myEventManager : MonoBehaviour
{
    //public EventNode.BranchType branch;
    [Tooltip("第一个场景是1第二个场景是2")]
    public int startID;
    public static myEventManager instance;
    public string interactionRecordBuffer;
    [SerializeField]
    private SimpleGraph graph;
    private EventNode startNode;
    private EventNode curNode;
    private myEventPlayer eventPlayer;
    bool isShow;
    //int a = 1;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            //虽然销毁操作的确能解决DontDestroyOnLoad物体重复出现的问题，但被销毁的物体们仍会执行完Awake操作
            if(instance != this)
                Destroy(this.gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;//切换场景其实会被触发两次
    }

    // Start is called before the first frame update
    void Start()
    {
        //寻找头节点
        for(int i = 0;i < graph.nodes.Count;i++)
        {
            EventNode node = graph.nodes[i] as EventNode;
            if(node.DialogID == startID)
                startNode = node;
        }
        curNode = startNode;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            if(!isShow)
            {
                eventPlayer.SetNextPlayableAsset(curNode.timeLineAsset);
                eventPlayer.PlayNext();
                isShow = true;
            }
            else
            {
                EventProceed(EventNode.BranchType.A,false);
            }
        if(Input.GetKeyDown(KeyCode.Backspace)) {
            stopplay();
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)//一切场景加载触发事件只要有订阅，一定要有撤销订阅
    {
        if(eventPlayer != null)
            eventPlayer = null;
        
        GameObject tl = GameObject.Find("TimeLine");
        if(tl != null)
            eventPlayer = tl.GetComponent<myEventPlayer>();
        if(eventPlayer == null)
            Debug.LogError("找不到TimeLine文件 或 找不到myEventPlayer");
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayNextEvent()
    {
        eventPlayer.StopCurrentAndPlayNext(curNode.timeLineAsset);
    }
    void stopplay()
    {
        eventPlayer.StopPlay();
    }
    public void EventProceed(EventNode.BranchType _type,bool finDialog)
    {
        if(curNode.GetPort("exit").Connection == null)//是否到头
        {
            Debug.Log("当前节点已无后继");
            return;
        }
        var connectionList = curNode.GetPort("exit").GetConnections();
        EventNode node;
        for (int i = 0; i < connectionList.Count; i++)
        {
            node = connectionList[i].node as EventNode;
            if(node.branch == _type)
            {
                interactionRecordBuffer += _type.ToString();
                //Debug.Log("curNode->next[" + _type + "]Node" + "::" + node.DialogID.ToString() + "|+|" + node.Name); 
                curNode = node;
                // eventPlayer.SetNextPlayableAsset(curNode.timeLineAsset);
                // if(!finDialog)
                //     eventPlayer.PlayNext();
                PlayNextEvent();
                return;
            }
        }
        Debug.LogError("找不到当前节点的"+ _type.ToString() + "分支");
    }
    public void EventProceed()
    {
        if(curNode.GetPort("exit").Connection == null)//是否到头
        {
            Debug.Log("【myEventSyetem】当前节点已无后继");
            return;
        }
        curNode = curNode.GetPort("exit").Connection.node as EventNode;
        PlayNextEvent();
    }

    public string GetCurNodeName()
    {
        return curNode.Name;
    }
}
