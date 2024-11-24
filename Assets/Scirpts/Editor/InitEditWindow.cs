using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
public class InitEditWindow : EditorWindow 
    {
        #region Prefab 路径
        enum PrefabType{
            Enemy,
            Loot,
            Etc
        }
        private const string enemyPrefabsPath = "Assets/Prefabs/new/Enemy/Actor/";
        private const string lootPrefabsPath = "Assets/Prefabs/new/Loot/";
        private const string etcPrefabsPath = "Assets/Prefabs/new/Etc/";
        private const string prefabSuffix = ".prefab";
        #endregion
        #region Scene 路径
        private const string srcFilePath = "Assets/Scenes/Model";
        private const string dstFilePath = "Assets/Scenes/TempleteScene";
        private const string fileSuffix = ".unity";
        #endregion
        private int stageDefaultCount = 2;//stage default count
        private const int MAX_STAGE_COUNT = 5;
        private List<string> stageGrids = new List<string>();
        int selectedStageId = 0;
        int selectedPrefabId = 0;
        int selectedGeneratePanelId = 0;//0==enemy 1==loot 2==etc
        private List<string> enemyPrefabsList = new List<string>();
        private List<string> lootPrefabsList = new List<string>();
        private List<string> etcPrefabsList = new List<string>();
        private List<List<string>> generatePanelList = new List<List<string>>();
        private bool activeGenerateMode;
        private void OnEnable() {
            ResetStage();
            LoadGeneratePanelList();
            SceneView.duringSceneGui += OnSceneGUI;
        }

    

    private void OnDestroy() {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        private GameObject levelObject;
        private GameObject LevelObject{
            get{
                if(levelObject == null){
                    levelObject = GameObject.Find("LevelObject");
                    if(levelObject == null){
                        levelObject = new GameObject("LevelObject");
                        levelObject.AddComponent<LevelObject>();
                    }
                    return levelObject;
                }else{
                    return levelObject;
                }
            }
        }


        [MenuItem("TimeOfHeart/DevPanel")]
        private static void ShowWindow() 
        {
            var window = GetWindow(typeof(InitEditWindow));
            window.titleContent = new GUIContent("DevPanel");
            window.Show();
        }

        private void OnGUI() 
        {
            GUILayout.Space(10);
            #region 初始化
            if(GUILayout.Button("Generate Template Scene"))
            {
                //Logic
                string fileName = dstFilePath;
                while(File.Exists(fileName + fileSuffix)){
                    fileName += "(1)";
                }
                File.Copy(srcFilePath + fileSuffix, fileName + fileSuffix, true);
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);
            GUILayout.Label("模板Scene源路径:" + srcFilePath);
            GUILayout.Space(10);
            GUILayout.Label("模板Scene生成路径:" + dstFilePath);
            #endregion

            #region StageID
            //Stage ID Button 1->5
            GUILayout.Space(20);//空格,没什么好说的
            GUILayout.Label("Stage 选项");
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal("Box");
            if(GUILayout.Button("Add Stage")){
                AddStage();
            }
            if(GUILayout.Button("Remove Stage")){
                RemoveStage();
            }
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Reset Stage")){
                ResetStage();
            }
            GUILayout.Space(10);
            GUILayout.Label("选定 Stage ID");
            selectedStageId = GUILayout.SelectionGrid(selectedStageId,stageGrids.ToArray(), stageGrids.Count);
            //<WARNING> 上面这条selectedStageId永远是从0到4
            
            GUILayout.BeginHorizontal("Box");
            if(GUILayout.Button("Show All Stage")){
                ShowAllStage();
            }
            if(GUILayout.Button("Hide Stage")){
                HideStage();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndVertical();
            #endregion
            
            #region  Prefab 生成
            GUILayout.Space(10f);
            GUILayout.Label("Prefab 生成");
            GUILayout.BeginHorizontal("Box");
            activeGenerateMode = GUILayout.Toggle(activeGenerateMode,new GUIContent("Active Generate Mode","用完记得退出!!!"));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            //display all prefab
            GUILayout.BeginVertical("Box");
                // GUILayout.BeginHorizontal("Box");
                //     if(GUILayout.Button("Add Prefab")){
                //         AddPrefab();
                //     }
                //     if(GUILayout.Button("Remove Prefab")){
                //         RemovePrefab();
                //     }
                // GUILayout.EndHorizontal();
                GUILayout.Space(10);
                selectedGeneratePanelId = GUILayout.SelectionGrid(selectedGeneratePanelId,new[]{"ActorEnemy","Loot","Etc."},3);
                GUILayout.Label("Prefab List");
                GUILayout.Space(5);
                if(selectedGeneratePanelId == 0){
                    GUILayout.Label("Enemy");
                    GUILayout.BeginVertical();
                    selectedPrefabId = GUILayout.SelectionGrid(selectedPrefabId, enemyPrefabsList.ToArray(), 2);
                    GUILayout.EndVertical();
                }else if(selectedGeneratePanelId == 1){
                    GUILayout.Label("Loot");
                    GUILayout.BeginVertical();
                    selectedPrefabId = GUILayout.SelectionGrid(selectedPrefabId, lootPrefabsList.ToArray(), 2);
                    GUILayout.EndVertical();
                }else{
                    GUILayout.Label("Etc.");
                    GUILayout.BeginVertical();
                    selectedPrefabId = GUILayout.SelectionGrid(selectedPrefabId, etcPrefabsList.ToArray(), 2);
                    GUILayout.EndVertical();
                }
            GUILayout.EndVertical();

            #endregion
            
            if (GUILayout.Button("Test Button"))
            {
                TraversePrefabsList();
            }
            //Select Prefab: Enemy,Loot,TransGate
            
        }
        
        private void OnSceneGUI(SceneView sceneView) {
            if(activeGenerateMode){
                //禁用鼠标选取功能
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                //if press mouse left button
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    //get the world coordinates of the curosr
                    Vector3 mousePos = Event.current.mousePosition;
                    // mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
                    Vector3 worldPoint = sceneView.camera.ScreenToWorldPoint(HandleUtility.GUIPointToScreenPixelCoordinate(mousePos));
                    worldPoint.z = 0f;
                    //generate a cube in this coordinates
                    GeneratePrefab(GetCurStageTrans(selectedStageId),worldPoint,selectedGeneratePanelId,selectedPrefabId);
                }
            }
            
        }
        

        #region 方法
        private void ShowAllStage()
        {
            for(int i = 0; i < stageGrids.Count; i++){
                Transform stageObj = GetCurStageTrans(i);
                if(stageObj != null){
                    stageObj.gameObject.SetActive(true);
                }
            }
        }

        private void HideStage()
        {
            for(int i = 1; i < stageGrids.Count; i++){
                Transform stageObj = GetCurStageTrans(i);
                if(stageObj != null){
                    stageObj.gameObject.SetActive(false);
                }
            }
        }
        private Transform GetCurStageTrans(int selectedStageId){
            if(LevelObject != null){
                Transform stageObj = LevelObject.transform.Find("Stage" + (selectedStageId + 1).ToString());
                if(stageObj!=null){
                    return stageObj;
                }else{
                    return null;
                }
            }
            return null;
        }
        
        private void GeneratePrefab(Transform parent, Vector3 generatePosition,int selectedGeneratePanelId,int selectedPrefabId)
        {
            string loadPath = GetGeneratePanelPath(selectedGeneratePanelId) + generatePanelList[selectedGeneratePanelId][selectedPrefabId] + prefabSuffix;

            // 使用AssetDatabase.LoadAssetAtPath加载游戏物体
            GameObject loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(loadPath);
            Object @object;
            if(selectedGeneratePanelId == (int)PrefabType.Enemy || selectedGeneratePanelId == (int)PrefabType.Loot){
                 @object = PrefabUtility.InstantiatePrefab(loadedGameObject, parent);
            }else
            {
                @object = PrefabUtility.InstantiatePrefab(loadedGameObject, null);
            }
            Undo.RegisterCreatedObjectUndo(@object, loadedGameObject.name);
            GameObject gameObject = @object as GameObject;
            gameObject.transform.position = generatePosition;
        }

        private void AddStage()
        {
            if(stageGrids.Count >= MAX_STAGE_COUNT)
            {
                return;
            }
            stageGrids.Add((stageGrids.Count + 1).ToString());//count += 1
            if(LevelObject != null){
                Transform stageObj = LevelObject.transform.Find("Stage" + stageGrids.Count.ToString());
                if(stageObj==null){
                    stageObj = new GameObject("Stage" + stageGrids.Count.ToString()).transform;
                    stageObj.SetParent(LevelObject.transform);
                    stageObj.AddComponent<StageObject>().myStageID = (StageID)(stageGrids.Count - 1);
                }
            }
        }

        private void RemoveStage()
        {
            if(stageGrids.Count <= 1)
            {
                return;
            }
            stageGrids.RemoveAt(stageGrids.Count - 1);//count -= 1
            if(LevelObject != null){
                Transform stageObj = LevelObject.transform.Find("Stage" + (stageGrids.Count + 1).ToString());
                if(stageObj!=null){
                    Undo.DestroyObjectImmediate(stageObj.gameObject);
                }
            }
        }

        private void ResetStage()
        {
            if(LevelObject != null){
                stageGrids.Clear();
                for(int i = 0;i<LevelObject.transform.childCount;i++){
                    stageGrids.Add((i + 1).ToString());//count += 1
                }
            }
        }

        private void LoadEnemyPrefabsPath()
        {
            enemyPrefabsList.Clear();
            string[] prefabs = Directory.GetFiles(enemyPrefabsPath, "*.prefab");
            foreach (string prefab in prefabs)
            {
                string fileName = Path.GetFileNameWithoutExtension(prefab);
                enemyPrefabsList.Add(fileName);
            }
        }

        private void TraversePrefabsList()
        {
            foreach (string prefab in enemyPrefabsList)
            {
                Debug.Log(prefab);
            }
        }

        private void LoadLootPrefabsPath()
        {
            lootPrefabsList.Clear();
            string[] prefabs = Directory.GetFiles(lootPrefabsPath, "*.prefab");
            foreach (string prefab in prefabs)
            {
                string fileName = Path.GetFileNameWithoutExtension(prefab);
                lootPrefabsList.Add(fileName);
            }
        }
        private void LoadEtcPrefabsPath()
        {
            etcPrefabsList.Clear();
            string[] prefabs = Directory.GetFiles(etcPrefabsPath, "*.prefab");
            foreach (string prefab in prefabs)
            {
                string fileName = Path.GetFileNameWithoutExtension(prefab);
                etcPrefabsList.Add(fileName);
            }
        }
        private void LoadGeneratePanelList()
        {
            LoadEnemyPrefabsPath();
            LoadLootPrefabsPath();
            LoadEtcPrefabsPath();
            generatePanelList.Add(enemyPrefabsList);
            generatePanelList.Add(lootPrefabsList);
            generatePanelList.Add(etcPrefabsList);
        }

        private string GetGeneratePanelPath(int selectedGeneratePanelId)
        {
            if(selectedGeneratePanelId == 0){
                return enemyPrefabsPath;
            }else if(selectedGeneratePanelId == 1){
                return lootPrefabsPath;
            }else{
                return etcPrefabsPath;
            }
        }
        #endregion
    }
    