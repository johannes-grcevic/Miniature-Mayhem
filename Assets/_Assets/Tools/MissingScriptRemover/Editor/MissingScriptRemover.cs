using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SetaMissingScriptRemover
{
    public class MissingScriptRemover : EditorWindow
    {
        private List<GameObject> prefabsWithMissingScripts = new List<GameObject>();
        private List<GameObject> objectsWithMissingScripts = new List<GameObject>();
        private List<bool> selectedObjects = new List<bool>();
        private Vector2 scrollPosPrefab;
        private Vector2 scrollPosScene;
        private Texture2D logo;
        private int selectedTab = 0;


        [MenuItem("Tools/Seta/Missing Script Remover")]
        public static void ShowWindow()
        {
            MissingScriptRemover window = GetWindow<MissingScriptRemover>("Missing Script Remover");
            window.minSize = new Vector2(350, 580);
        }

        private void OnEnable()
        {
            string scriptPath = System.IO.Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromScriptableObject(this)));
            string logoPath = System.IO.Path.Combine(scriptPath, "Logo", "removerlogo.png");

            logo = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
            EditorApplication.update += UpdateMissingScripts;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateMissingScripts;
        }

        private void OnGUI()
        {
            if (logo != null)
            {
                GUIStyle logoStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter
                };

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logo, GUILayout.Width(200), GUILayout.Height(100));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Logo not found!", EditorStyles.boldLabel);
            }

            GUILayout.Space(10);
            GUIStyle centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Find \"Missing Scripts\" in:", centeredLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            string[] tabNames = { "Scene", "Project" };
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);

            if (selectedTab == 0)
            {
                DrawSceneTab();
            }
            else
            {
                DrawProjectTab();
            }
        }

        private void DrawSceneTab()
        {
            if (GUILayout.Button("Find \"Missing Scripts\" in Scene"))
            {
                FindInCurrentScene();
            }

            GUILayout.Label($"Objects with \"Missing Scripts\" in Scene: {objectsWithMissingScripts.Count}", EditorStyles.boldLabel);
            scrollPosScene = EditorGUILayout.BeginScrollView(scrollPosScene, GUILayout.Height(150));

            if (objectsWithMissingScripts.Count > 0)
            {
                for (int i = 0; i < objectsWithMissingScripts.Count; i++)
                {
                    GameObject go = objectsWithMissingScripts[i];

                    if (i >= selectedObjects.Count)
                    {
                        selectedObjects.Add(false);
                    }

                    GUILayout.BeginHorizontal();
                    selectedObjects[i] = GUILayout.Toggle(selectedObjects[i], "", GUILayout.Width(20));

                    if (GUILayout.Button(go.name, GUILayout.ExpandWidth(true)))
                    {
                        Selection.activeGameObject = go;
                        EditorGUIUtility.PingObject(go);
                    }

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        objectsWithMissingScripts.RemoveAt(i);
                        selectedObjects.RemoveAt(i);
                        i--;
                        GUILayout.EndHorizontal();
                        continue;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No objects with \"Missing Scripts\" found in scene.");
            }

            EditorGUILayout.EndScrollView();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button("Remove \"Missing Script\" from Selected"))
            {
                RemoveMissingScriptFromSelectedObject();
            }

            GUI.backgroundColor = Color.white;
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Remove \"Missing Script\" from All Objects in Scene"))
            {
                RemoveMissingScriptsFromAllObjects();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Clear Missing Scripts List"))
            {
                ClearMissingScriptsList();
            }

            GUILayout.Space(10);
            GUILayout.Label("Instructions:", EditorStyles.boldLabel);
            GUILayout.Label("✓ - determines if the 'Missing Script' in object should be removed using 'Remove from Selected'.", EditorStyles.wordWrappedLabel);
            GUILayout.Label("'Object name' - select the object in the Hierarchy window.", EditorStyles.wordWrappedLabel);
            GUILayout.Label("X - removes the object from the list.", EditorStyles.wordWrappedLabel);
        }

        private void DrawProjectTab()
        {
            if (GUILayout.Button("Find Prefabs with Missing Scripts in Project"))
            {
                FindPrefabsWithMissingScripts();
            }

            GUILayout.Label($"Prefabs with \"Missing Scripts\" in Project: {prefabsWithMissingScripts.Count}", EditorStyles.boldLabel);
            scrollPosPrefab = EditorGUILayout.BeginScrollView(scrollPosPrefab, GUILayout.Height(150));

            if (prefabsWithMissingScripts.Count > 0)
            {
                for (int i = 0; i < prefabsWithMissingScripts.Count; i++)
                {
                    GameObject prefab = prefabsWithMissingScripts[i];
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        prefabsWithMissingScripts.RemoveAt(i);
                        i--;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No prefabs with \"Missing Scripts\" found in project.");
            }

            EditorGUILayout.EndScrollView();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Remove \"Missing Scripts\" from all Prefabs in Project"))
            {
                ProcessPrefabs();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Clear Prefab List"))
            {
                ClearPrefabList();
            }
        }

        private void UpdateMissingScripts()
        {
            for (int i = objectsWithMissingScripts.Count - 1; i >= 0; i--)
            {
                if (objectsWithMissingScripts[i] == null)
                {
                    objectsWithMissingScripts.RemoveAt(i);
                    selectedObjects.RemoveAt(i);
                }
            }
        }

        private void FindInCurrentScene()
        {
            objectsWithMissingScripts.Clear();
            selectedObjects.Clear();
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            foreach (GameObject go in allObjects)
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject))
                {
                    UnityEngine.Component[] components = go.GetComponents<UnityEngine.Component>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == null)
                        {
                            objectsWithMissingScripts.Add(go);
                            selectedObjects.Add(false);
                            break;
                        }
                    }
                }
            }
        }

        private void RemoveMissingScriptFromSelectedObject()
        {
            for (int i = selectedObjects.Count - 1; i >= 0; i--)
            {
                if (selectedObjects[i])
                {
                    GameObject go = objectsWithMissingScripts[i];
                    var components = go.GetComponents<UnityEngine.Component>();

                    for (int j = components.Length - 1; j >= 0; j--)
                    {
                        if (components[j] == null)
                        {
                            UnityEditor.Undo.RegisterCompleteObjectUndo(go, "Remove Missing Script");
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                            break;
                        }
                    }
                    objectsWithMissingScripts.RemoveAt(i);
                    selectedObjects.RemoveAt(i);
                }
            }
        }

        private void RemoveMissingScriptsFromAllObjects()
        {
            foreach (GameObject go in objectsWithMissingScripts)
            {
                if (go != null)
                {
                    var components = go.GetComponents<UnityEngine.Component>();
                    for (int i = components.Length - 1; i >= 0; i--)
                    {
                        if (components[i] == null)
                        {
                            UnityEditor.Undo.RegisterCompleteObjectUndo(go, "Remove Missing Script");
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        }
                    }
                }
            }
            FindInCurrentScene();
        }

        private void FindPrefabsWithMissingScripts()
        {
            prefabsWithMissingScripts.Clear();
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    var components = prefab.GetComponentsInChildren<UnityEngine.Component>(true);
                    foreach (var component in components)
                    {
                        if (component == null)
                        {
                            prefabsWithMissingScripts.Add(prefab);
                            break;
                        }
                    }
                }
            }
        }

        private void ProcessPrefabs()
        {
            List<GameObject> prefabsToRemove = new List<GameObject>();

            foreach (var prefab in prefabsWithMissingScripts)
            {
                if (prefab == null) continue;

                GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                if (prefabInstance == null) continue;

                RemoveMissingScripts(prefabInstance);

                PrefabUtility.SaveAsPrefabAsset(prefabInstance, AssetDatabase.GetAssetPath(prefab));
                DestroyImmediate(prefabInstance);
                prefabsToRemove.Add(prefab);
            }

            foreach (var prefab in prefabsToRemove)
            {
                prefabsWithMissingScripts.Remove(prefab);
            }
        }

        private void RemoveMissingScripts(GameObject go)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            foreach (Transform child in go.transform)
            {
                RemoveMissingScripts(child.gameObject);
            }
        }

        private void ClearMissingScriptsList()
        {
            objectsWithMissingScripts.Clear();
            selectedObjects.Clear();
        }

        private void ClearPrefabList()
        {
            prefabsWithMissingScripts.Clear();
        }
    }
}