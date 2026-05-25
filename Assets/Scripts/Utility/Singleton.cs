using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance {  get; private set; }

    [SerializeField]
    private bool isSingleton = false;

    [SerializeField]
    private bool isPersistant = false;

    protected virtual void Awake()
    {
        if (isSingleton)
        {
            // Check if instance already exists
            if (!Instance)
            {
                // Set initial instance
                Instance = this as T;
            }
            else
            {
                // We don't want additional instances
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            Instance = this as T;
        }

        // Check if we want to persist this gameObject between loads
        if (isPersistant)
        {
            // Make sure this object stays on reloads
            DontDestroyOnLoad(gameObject);
        }
    }
}