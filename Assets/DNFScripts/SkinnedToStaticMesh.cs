using UnityEngine;

public class SkinnedToStaticMesh : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public string signName;

    public void ConvertAndSave()
    {
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        // Save as Prefab
#if UNITY_EDITOR
        
        string meshPath = "Assets/Resources/Prefabs/Hand Signs/" + signName + ".asset";
        UnityEditor.AssetDatabase.CreateAsset(bakedMesh, meshPath);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}
