using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GaussianSplatting.Runtime;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GaussianSplatSequencer))]
public class GaussianSplatSequencerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var sequencer = target as GaussianSplatSequencer;
        
        if(GUILayout.Button("Load Assets")){
            var path = EditorUtility.OpenFolderPanel("Select Assets Folder", "", "");
            path = path.Replace(Application.dataPath, "Assets");
            Debug.Log(path);

            var assetPaths = AssetDatabase.FindAssets("t:GaussianSplatAsset", new string[]{path});
            if(string.IsNullOrEmpty(path) || assetPaths.Length == 0){
                Debug.Log("No assets found");
                return;
            }
            Debug.Log(assetPaths.Length);
            sequencer.assetsSequence = new GaussianSplatAsset[assetPaths.Length];
            for(int i = 0; i < assetPaths.Length; i++){
                var assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
                sequencer.assetsSequence[i] = AssetDatabase.LoadAssetAtPath<GaussianSplatAsset>(assetPath);
            }
            
        }
    }
}

#endif


public class GaussianSplatSequencer : MonoBehaviour
{
    public GaussianSplatRenderer splatRenderer;
    public GaussianSplatAsset[] assetsSequence;
    public float targetFPS = 12f;

    float timeBetweenAssets => 1f / targetFPS;

    private int currentAssetIndex = 0;
    private float timeSinceLastAsset = 0f;


    void Start(){
        currentAssetIndex = 0;
        timeSinceLastAsset = 0f;
        splatRenderer.m_Asset = assetsSequence[currentAssetIndex];        
    }
    void FixedUpdate()
    {
        timeSinceLastAsset += Time.deltaTime;
        if(timeSinceLastAsset > timeBetweenAssets){
            timeSinceLastAsset = 0f;
            currentAssetIndex++;
            if(currentAssetIndex >= assetsSequence.Length){
                currentAssetIndex = 0;
            }
            splatRenderer.m_Asset = assetsSequence[currentAssetIndex];
        }
    }
}
