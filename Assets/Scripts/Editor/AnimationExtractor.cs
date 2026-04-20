using UnityEditor;
using UnityEngine;
using System.IO;

public class AnimationExtractor : EditorWindow
{
    [MenuItem("Assets/Extract All Animations")]
    public static void Extract()
    {
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            // Load all assets inside the FBX (clips are sub-assets)
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var asset in subAssets)
            {
                if (asset is AnimationClip clip && !asset.name.Contains("__preview__"))
                {
                    AnimationClip originalClip = clip;
                    AnimationClip newClip = new();

                    // Copy all settings and curves
                    EditorUtility.CopySerialized(originalClip, newClip);

                    string folder = Path.GetDirectoryName(path);
                    string newPath = Path.Combine(folder, originalClip.name + ".anim");

                    AssetDatabase.CreateAsset(newClip, newPath);
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}