using UnityEngine;

namespace Utils
{
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : SingletonMonoBehavior<T>
    {
        [SerializeField, Tooltip("Should this instance persist across scenes?")]
        private bool persistAcrossScenes = false;
        
        [SerializeField, Tooltip("Automatically create an instance if one does not exist.")]
        private bool autoCreateInstance = false;

        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _isShuttingDown = false;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }
                
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null && InstanceShouldAutoCreate())
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";
                            
                            // Now the cast is safe, since T is guaranteed to inherit from SingletonMonoBehavior<T>
                            if (_instance.persistAcrossScenes)
                            {
                                DontDestroyOnLoad(singletonObject);
                            }
                        }
                        else if (_instance == null)
                        {
                            Debug.LogError($"[Singleton] An instance of {typeof(T)} is needed in the scene, but there is none.");
                        }
                    }
                    return _instance;
                }
            }
        }

        private static bool InstanceShouldAutoCreate()
        {
            T existing = FindObjectOfType<T>();
            if (existing != null)
            {
                return existing.autoCreateInstance;
            }
            return false;
        }

        protected virtual void Awake()
        {
            if (_isShuttingDown)
            {
                Destroy(gameObject);
                return;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this as T;

                    if (persistAcrossScenes)
                    {
                        DontDestroyOnLoad(gameObject);
                    }
                    InitializeSingleton();
                }
                else if (_instance != this)
                {
                    Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} found. Destroying duplicate attached to {gameObject.name}.");
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void InitializeSingleton()
        {
            // Override this in a subclass to add initialization logic.
        }

        protected virtual void OnApplicationQuit()
        {
            _isShuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _isShuttingDown = true;
            }
        }
    }
}
