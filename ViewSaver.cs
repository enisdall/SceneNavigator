// This script written by Enis DAL.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ViewSaver : EditorWindow
{
    public UnityEditor.SceneView sceneView;
    public List<Vector3> savedPos = new List<Vector3>();
    public List<Quaternion> savedRot = new List<Quaternion>();
    public List<GameObject> cameras = new List<GameObject>();
    public List<GUIContent> guiContent = new List<GUIContent>();
    public List<string> posNames = new List<string>();  
    Vector2 scrollPos;

    private void Awake()
    {     
        sceneView = UnityEditor.SceneView.lastActiveSceneView;

        savedPos.Clear();
        savedRot.Clear();

        for (int i = 0; i < cameras.Count; i++)
        {
            DestroyImmediate(cameras[i]);
        }

        cameras.Clear();
        guiContent.Clear();
    }

    [MenuItem("Window/View Saver")]
    static void Init()
    {
        ViewSaver window = (ViewSaver)EditorWindow.GetWindow(typeof(ViewSaver));
        window.maxSize = new Vector2(200,window.maxSize.y);
        window.Show();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 1.1f));
        UpdateList();

        EditorGUILayout.EndScrollView();
        GUILayout.FlexibleSpace();
        GUI.color = Color.green;

        if (GUILayout.Button("Add"))
        {
            AddVector();
            CreateCamera();

        }

    }

    public void AddVector()
    {
        if (sceneView != null)
        {
            savedPos.Add(sceneView.pivot);
            savedRot.Add(sceneView.rotation);
        }

    }

    public void CreateCamera()
    {
        GameObject cam = new GameObject();
        cam.AddComponent<Camera>();
        cameras.Add(cam);

        cam.transform.position = sceneView.camera.transform.position;
        cam.transform.rotation = sceneView.rotation;
        cam.name = "New View";
        cam.hideFlags = HideFlags.HideInHierarchy;

        string str = "New View";
        posNames.Add(str);

        RenderTexture renderTex = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);

        if (renderTex != null)
        {
            renderTex.Create();
            cam.GetComponent<Camera>().targetTexture = renderTex;
            GUIContent guicont = new GUIContent("", renderTex);
            guiContent.Add(guicont);
        }

    }

    public void UpdateList()
    {
        for (int i = 0; i < savedPos.Count; i++)
        {
            GUI.color = Color.white;

            if (GUILayout.Button(guiContent[i], GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 4)))
            {
                GoToView(i);
            }

            GUILayout.BeginHorizontal();
            posNames[i] = GUILayout.TextField(posNames[i]);
            GUI.color = Color.red;

            if (GUILayout.Button("D", GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 48)))
            {
                DeleteView(i);
            }

            GUI.color = Color.red;
            GUILayout.EndVertical();

        }

    }

    public void GoToView(int index)
    {
        sceneView.pivot = savedPos[index];
        sceneView.rotation = savedRot[index];
    }

    public void DeleteView(int index)
    {
        savedPos.RemoveAt(index);
        savedRot.RemoveAt(index);
        DestroyImmediate(cameras[index]);
        cameras.RemoveAt(index);
        guiContent.RemoveAt(index);

    }

}


