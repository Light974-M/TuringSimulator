using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    /// <summary>
    /// create a singleton with unique and accessible instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : UPDBBehaviour where T : Component
    {
        /// <summary>
        /// tell if this singleton has to Hide in inspector
        /// </summary>
        protected bool hideInInspector = false;


        #region Public API

        public bool HideInInspector
        {
            get => hideInInspector; 
            set => hideInInspector = value;
        }

        #endregion

        #region Singleton Initialize

        /// <summary>
        /// unique instance of class
        /// </summary>
        protected static T _instance = null;

        public static T Instance
        {
            get
            {
                //if instance is null and no instance was found in scene, create a new obj to contain singleton
                if (_instance == null && !TryFindObjectOfType(out _instance))
                {
                    GameObject obj = new GameObject($"{typeof(T).Name}");
                    _instance = obj.AddComponent<T>();
                    Debug.LogWarning("[Singleton] Created new Singleton Object " + typeof(T).Name);
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// awake is called when script instance is being loaded
        /// </summary>
        protected virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// called when script instance is being destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (hideInInspector)
                hideFlags = HideFlags.HideInInspector;
            else
                hideFlags = HideFlags.None;
        }

        /*********************************CUSTOM METHODS**********************************/

        protected override void OnScene()
        {
            Initialize();
        }

        /// <summary>
        /// called at awake and on draw gizmos, make sure there is only one instance of class loaded in scene
        /// </summary>
        protected void Initialize()
        {
            if (_instance == null)
                _instance = this as T;
            else if (_instance != this as T)
                IntelliDestroy(this);
        }
    }
}
