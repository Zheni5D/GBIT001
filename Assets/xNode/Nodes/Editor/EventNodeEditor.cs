using XNodeEditor;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

[CustomNodeEditor(typeof (EventNode))]
public class eventNodeEditor : NodeEditor
{
    private bool m_showUI = true;
    private EventNode evNode;
    public override void OnBodyGUI()
    {
        //隐藏面板功能
        //-----------------
        if(GUILayout.Button("Hide/Show"))
        {
            m_showUI = !m_showUI;
        }

        if(!m_showUI)
            return;

        //自定义绘制Body
        //------------------
        //base.OnBodyGUI();
        if(evNode == null)
            evNode  = target as EventNode;
        
        //开始绘制
        GUILayout.BeginVertical();
        NodeEditorGUILayout.PortField(new GUIContent("enter"), target.GetInputPort("enter"), GUILayout.MinWidth(0));
        NodeEditorGUILayout.PortField(new GUIContent("exit"), target.GetOutputPort("exit"), GUILayout.MinWidth(0));
        GUILayout.EndVertical();

        //绘制公共变量
        evNode.DialogID = EditorGUILayout.IntField("DialogID", evNode.DialogID);
        evNode.Name = EditorGUILayout.TextField("Name", evNode.Name);
        evNode.branch = (EventNode.BranchType) EditorGUILayout.EnumPopup("branch", evNode.branch, GUILayout.ExpandWidth(true));
        //绘制playable asset
        GUILayout.Space(10);
        GUILayout.Label("TimeLine资源", GUILayout.Width(100), GUILayout.Height(20)); 
        evNode.timeLineAsset = EditorGUILayout.ObjectField(evNode.timeLineAsset, typeof(PlayableAsset), true) as PlayableAsset;
        GUILayout.Space(10);
            

    }
}
