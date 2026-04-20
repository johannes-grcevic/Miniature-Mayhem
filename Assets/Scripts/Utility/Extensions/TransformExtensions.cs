using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindChildWithTag(this Transform parent, string tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag(tag))
            {
                return child;
            }

            Transform result = FindChildWithTag(child, tag);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    public static Transform FindInChildren(this Transform parent, string name)
    {
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child;
            }

            Transform result = FindInChildren(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    public static bool CompareTagParent(this Transform transform, string tag)
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            if (parent.CompareTag(tag))
            {
                return true;
            }
            parent = parent.parent;
        }
        return false;
    }
}
