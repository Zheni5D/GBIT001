using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(EnemyAIMonitor))]
public class EnemyAIMonitorEditor : Editor {
    private bool activeAddPatrolMode = false;
    private List<Transform> patrolPointsList = new List<Transform>();
    private Transform[] patrolPointsArray;
    private const string timeCrackPath = "Assets/Prefabs/new/Enemy/TimeCrack.prefab";
    private const string shieldPath = "Assets/Prefabs/new/Enemy/Shield.prefab";
    private 
    enum FSMState{
        simpleFSM,
        actorFSM,
        standFSM//not implemented yet
    }
    private int fsmStateId = 0;//simpleFSM=0,actorFSM=1,standFSM=2
    private bool _hasTimeCrack = false;
    private bool _hasShield = false;
    private bool _hasFlashSpeed;
    private bool _hasBodyExplosion;
    private bool _hasGhostMode;
    private Color _outlineColor;
    
    private void OnEnable() {
        LoadPatrolPointsList();
        SceneView.duringSceneGui += OnSceneGUI;
        EnemyAIMonitor ctr = target as EnemyAIMonitor;
        ctr.Refresh();
        simpleFSM fsm = ctr.GetFSM();
        // ctr.outlineColor = _outlineColor;
        //fsm is actorFSM
        if(fsm is actorFSM)
        {
            fsmStateId = 1;
        }
        //fsm is simpleFSM
        else if(fsm is simpleFSM)
        {
            fsmStateId = 0;
        }
        //fsm is standFSM
        _hasShield = ctr._hasShield;
        _hasTimeCrack = ctr._hasTimeCrack;
        _hasFlashSpeed = ctr._hasFlashSpeed;
        _hasGhostMode = ctr._hasGhostMode;
        _hasBodyExplosion = ctr._hasBodyExplosion;
    }
    private void OnDestroy() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("设置描边颜色",GUILayout.MaxWidth(100f)))
        {
            var s = target as EnemyAIMonitor;
            var rend = s.GetComponent<Renderer>();
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", s.outlineColor);
            rend.SetPropertyBlock(propertyBlock);
            
            EditorUtility.SetDirty(s);
            Undo.RecordObject(s,"set outline color");
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        if(GUILayout.Button("Refresh",GUILayout.MaxWidth(55f)))
        {
            EnemyAIMonitor ctr = target as EnemyAIMonitor;
            ctr.Refresh();
            simpleFSM fsm = ctr.GetFSM();
            //fsm is actorFSM
            if(fsm is actorFSM)
            {
                fsmStateId = 1;
            }
            //fsm is simpleFSM
            else if(fsm is simpleFSM)
            {
                fsmStateId = 0;
            }
            //fsm is standFSM
        }
        #region FSM切换
        if(fsmStateId == (int)FSMState.actorFSM){
            GUILayout.Label("FSM Type: actorFSM");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal("box");
            if(GUILayout.Button("Switch to simpleFSM",GUILayout.Width(150f),GUILayout.Height(30f)))
            {
                //Logic
                EnemyAIMonitor ctr = target as EnemyAIMonitor;
                actorFSM initFSM = (actorFSM)ctr.GetFSM();
                simpleFSM targetFSM = ctr.AddComponent<simpleFSM>();

                var source = new SerializedObject(initFSM);
                var dstion = new SerializedObject(targetFSM);
                dstion.CopyFromSerializedProperty(source.FindProperty("paramater"));//将 SerializedProperty 中的值复制到序列化对象上的相应序列化属性。
                
                DestroyImmediate(initFSM);
                //Undo.DestroyObjectImmediate(initFSM);
                //source.Dispose();
                EditorUtility.SetDirty(ctr);
                ctr.Refresh();
                dstion.ApplyModifiedProperties();
                

                //Logic
                // serializedObject.FindProperty("Name").stringValue = "Codinggamer";
                // serializedObject.ApplyModifiedProperties();
                fsmStateId = 0;
            }
            GUILayout.Space(20);
            if(GUILayout.Button("Switch to standFSM",GUI.skin.customStyles[154],GUILayout.Width(150f),GUILayout.Height(30f)))
            {
                //not implemented yet
                // fsmStateId = 2;
            }
            GUILayout.EndHorizontal();
        }else if(fsmStateId == (int)FSMState.simpleFSM)
        {
            GUILayout.Label("FSM Type: simpleFSM");
            GUILayout.Space(20);
            GUILayout.BeginHorizontal("box");
            if(GUILayout.Button("Switch to actorFSM",GUILayout.Width(150f),GUILayout.Height(30f)))
            {
                //Logic
                EnemyAIMonitor ctr = target as EnemyAIMonitor;
                simpleFSM initFSM = ctr.GetFSM();
                actorFSM targetFSM = ctr.AddComponent<actorFSM>();

                var source = new SerializedObject(initFSM);
                var dstion = new SerializedObject(targetFSM);
                dstion.CopyFromSerializedProperty(source.FindProperty("paramater"));//将 SerializedProperty 中的值复制到序列化对象上的相应序列化属性。

                DestroyImmediate(initFSM);
                EditorUtility.SetDirty(ctr);
                ctr.Refresh();
                dstion.ApplyModifiedProperties();

                //Logic
                // serializedObject.FindProperty("Name").stringValue = "Codinggamer";
                // serializedObject.ApplyModifiedProperties();
                fsmStateId = 1;
            }
            GUILayout.Space(20);
            if(GUILayout.Button("Switch to standFSM", GUI.skin.customStyles[154],GUILayout.Width(100f),GUILayout.Height(30f)))
            {
                //not implemented yet
                // fsmStateId = 2;
            }
            GUILayout.EndHorizontal();
        }
        #endregion
        #region 添加巡逻点
        GUILayout.Space(10);
        GUILayout.Label("巡逻点配置");
        GUILayout.BeginHorizontal("Box");
        activeAddPatrolMode = GUILayout.Toggle(activeAddPatrolMode,new GUIContent("Active Modify Patrol Point Mode","用完记得退出!!!"));
        GUILayout.EndHorizontal();
        if(GUILayout.Button("重置巡逻点")){
            ClearPatrolPoint();
        }
        #endregion

        #region 特殊敌人属性设置
        GUILayout.Label("特殊敌人属性设置");
        #region 时空裂缝

        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal(); //开始水平布局
        if(GUILayout.Button("时空裂缝",GUILayout.MaxWidth(100f))){
            GameObject loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(timeCrackPath);
            EnemyAIMonitor ctl = target as EnemyAIMonitor;
            GameObject @object = PrefabUtility.InstantiatePrefab(loadedGameObject,ctl.transform) as GameObject;
            Undo.RegisterCreatedObjectUndo(@object, loadedGameObject.name);
            @object.transform.position = ctl.transform.position;
            _hasTimeCrack = true;
            ctl._hasTimeCrack = true;
            EditorUtility.SetDirty(ctl);
        }
        GUILayout.Space(50);
        if (_hasTimeCrack)
        {
            if(GUILayout.Button("删除",GUILayout.MaxWidth(100f))){
                EnemyAIMonitor ctl = target as EnemyAIMonitor;
                DestroyImmediate(ctl.transform.Find("TimeCrack").gameObject);
                _hasTimeCrack = false;
                ctl._hasTimeCrack = false;
                EditorUtility.SetDirty(ctl);
            }
        }
        GUILayout.Space(20);
        if(GUILayout.Button("刷新",GUILayout.MaxWidth(50f))){
            EnemyAIMonitor ctl = target as EnemyAIMonitor;
            _hasTimeCrack = ctl.transform.Find("TimeCrack") != null;
        }
        GUILayout.EndHorizontal(); //结束水平布局

        #endregion

        #region 盾牌

        GUILayout.Space(10);
        GUILayout.BeginHorizontal(); //开始水平布局
        if(GUILayout.Button("盾牌",GUILayout.MaxWidth(100f))){
            GameObject loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(shieldPath);
            EnemyAIMonitor ctl = target as EnemyAIMonitor;
            GameObject @object = PrefabUtility.InstantiatePrefab(loadedGameObject,ctl.transform) as GameObject;
            Undo.RegisterCreatedObjectUndo(@object, loadedGameObject.name);
            @object.transform.position = ctl.transform.position;
            _hasShield = true;
            ctl._hasShield = true;
            EditorUtility.SetDirty(ctl);
        }
        GUILayout.Space(50);
        if (_hasShield)
        {
            if(GUILayout.Button("删除",GUILayout.MaxWidth(100f))){
                EnemyAIMonitor ctl = target as EnemyAIMonitor;
                DestroyImmediate(ctl.transform.Find("Shield").gameObject);
                _hasShield = false;
                ctl._hasShield = false;
                EditorUtility.SetDirty(ctl);
            }
        }
        GUILayout.Space(20);
        if(GUILayout.Button("刷新",GUILayout.MaxWidth(50f))){
            EnemyAIMonitor ctl = target as EnemyAIMonitor;
            _hasShield = ctl.transform.Find("Shield") != null;
        }
        GUILayout.EndHorizontal(); //结束水平布局

        #endregion
        
        GUILayout.Space(10);
        if (!_hasFlashSpeed)
        {
            if(GUILayout.Button("飞毛腿",GUILayout.MaxWidth(100f))){
                _hasFlashSpeed = true;
                EnemyAIMonitor ctr = target as EnemyAIMonitor;
                ctr._hasFlashSpeed = true;
                var source = new SerializedObject(ctr.GetFSM());
                source.FindProperty("paramater").FindPropertyRelative("speedCoff").floatValue = 1.5f;
                var shadow = new SerializedObject(ctr.GetComponent<FollowShadow>());
                shadow.FindProperty("isGenerating").boolValue = true;
                EditorUtility.SetDirty(ctr);
                source.ApplyModifiedProperties();
                shadow.ApplyModifiedProperties();
            }
        }
        else
        {
            if(GUILayout.Button("取消飞毛腿",GUILayout.MaxWidth(100f))){
                _hasFlashSpeed = false;
                EnemyAIMonitor ctr = target as EnemyAIMonitor;
                ctr._hasFlashSpeed = false;
                var source = new SerializedObject(ctr.GetFSM());
                source.FindProperty("paramater").FindPropertyRelative("speedCoff").floatValue = 1f;
                var shadow = new SerializedObject(ctr.GetComponent<FollowShadow>());
                shadow.FindProperty("isGenerating").boolValue = true;
                EditorUtility.SetDirty(ctr);
                source.ApplyModifiedProperties();
                shadow.ApplyModifiedProperties();
            }
        }
        GUILayout.Space(10);
        if (!_hasBodyExplosion)
        {
            if(GUILayout.Button("尸爆",GUILayout.MaxWidth(100f))){
                _hasBodyExplosion = true;
                EnemyAIMonitor ctr = target as EnemyAIMonitor;
                ctr._hasFlashSpeed = true;
                var source = new SerializedObject(ctr.GetFSM());
                source.FindProperty("paramater").FindPropertyRelative("canBodyExplode").boolValue = true;
                EditorUtility.SetDirty(ctr);
                source.ApplyModifiedProperties();
            }
        }
        else
        {
            if(GUILayout.Button("取消尸爆",GUILayout.MaxWidth(100f))){
                _hasBodyExplosion = false;
                EnemyAIMonitor ctr = target as EnemyAIMonitor;
                ctr._hasFlashSpeed = false;
                var source = new SerializedObject(ctr.GetFSM());
                source.FindProperty("paramater").FindPropertyRelative("canBodyExplode").boolValue = false;
                EditorUtility.SetDirty(ctr);
                source.ApplyModifiedProperties();
            }
        }
        GUILayout.Space(10);
        if(GUILayout.Button("幽灵",GUILayout.MaxWidth(100f))){
            EnemyAIMonitor ctr = target as EnemyAIMonitor;
            _hasGhostMode = true;
            ctr._hasGhostMode = true;
            ctr.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,.4f);
            EditorUtility.SetDirty(ctr);
        }
        #endregion
        
        
    }

    private void OnSceneGUI(SceneView sceneView)
    {
         if(activeAddPatrolMode){
                //禁用鼠标选取功能
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                 //get the world coordinates of the curosr
                    Vector3 mousePos = Event.current.mousePosition;
                    // mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
                    Vector3 worldPoint = sceneView.camera.ScreenToWorldPoint(HandleUtility.GUIPointToScreenPixelCoordinate(mousePos));
                    worldPoint.z = 0f;
                    Handles.SphereHandleCap(  
                        GUIUtility.GetControlID(FocusType.Passive ) , 
                        worldPoint, Quaternion.identity, 1f , 
                        EventType.Repaint );
                //if press mouse left button CREATE
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                   AddPatrolPoint(worldPoint);
                }
                Handles.color = Color.red;
                foreach (var position in patrolPointsList)
                {
                    Handles.SphereHandleCap(  
                        GUIUtility.GetControlID(FocusType.Passive ) , 
                        position.position, Quaternion.identity, 1f , 
                        EventType.Repaint );
                }
                //draw lines between patrolPoints
                if(patrolPointsList.Count > 1)
                {
                    Handles.color = Color.red;
                    Vector3 lastPos = patrolPointsList[0].position;
                    for(int i = 1;i<patrolPointsList.Count;i++){
                        Vector3 curPos = patrolPointsList[i].position;
                        Handles.DrawLine(lastPos,curPos);
                        Handles.Label(curPos, i.ToString());
                        lastPos = curPos;
                    }
                }
            }
    }

    #region 方法
    private void LoadPatrolPointsList()
    {
        patrolPointsList.Clear();
        EnemyAIMonitor ctr = target as EnemyAIMonitor;
        patrolPointsList.Add(ctr.gameObject.transform);

        patrolPointsArray = ctr.GetFSM().paramater.portalTarget;
        if(patrolPointsArray.Length > 0)
        {
            foreach (var item in patrolPointsArray)
            {
                patrolPointsList.Add(item);
            }
        }
        // foreach (var item in patrolPointsList)
    }

    private void AddPatrolPoint(Vector3 pos)
    {
        EnemyAIMonitor ctr = target as EnemyAIMonitor;
        GameObject patrolRoot = GameObject.Find("PartolPoints");
        if(patrolRoot == null){
            patrolRoot = new GameObject("PartolPoints");
        }
        //create a new GameObject named PatrolPoint
        GameObject patrolPoint = new GameObject("PatrolPoint");
        patrolPoint.transform.position = pos;
        patrolPoint.transform.parent = patrolRoot.transform;
        patrolPointsList.Add(patrolPoint.transform);
        //get elements from patrolPointsList except first element
        List<Transform> tempList = new List<Transform>(patrolPointsList);
        tempList.RemoveAt(0);
        ctr.GetFSM().paramater.portalTarget = tempList.ToArray();
    }

    private void ClearPatrolPoint()
    {
        EnemyAIMonitor ctr = target as EnemyAIMonitor;

        for(int i = 1;i<patrolPointsList.Count;i++){
            DestroyImmediate(patrolPointsList[i].gameObject);
        }
        patrolPointsList.RemoveRange(1, patrolPointsList.Count - 1);

        var a = new SerializedObject(ctr.GetFSM());
        a.FindProperty("paramater.portalTarget").ClearArray();
        a.ApplyModifiedPropertiesWithoutUndo();
    }
    #endregion
}
