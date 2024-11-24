using UnityEditor;
using UnityEngine;

public class AddMenuItem
{

    // [MenuItem("MenuCommand/SwapGameObjectA")]
    // protected static void SwapGameObject()
    // {
    //     //只有两个物体才能交换
    //     if( Selection.gameObjects.Length == 2 )
    //     {
    //         Vector3 tmpPos = Selection.gameObjects[0].transform.position;
    //         Selection.gameObjects[0].transform.position = Selection.gameObjects[1].transform.position;
    //         Selection.gameObjects[1].transform.position = tmpPos;
    //         //处理两个以上的场景物体可以使用MarkSceneDirty
    //         UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() );
    //     }
    // }

    [MenuItem("TimeOfHeart/GetAssetPath")]
    protected static void GetAssetPath()
    {
        // 获取选中的资源路径
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(assetPath);
    }
}