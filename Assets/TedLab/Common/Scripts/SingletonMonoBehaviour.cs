using UnityEngine;
using System;

namespace TedLab
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get{
                if (instance == null) {
                    Type t = typeof(T);

                    instance = (T)FindObjectOfType (t);
                    if (instance == null) {
                        Debug.LogWarning (t + " 指定のコンポーネントを含むGameObjectがありません");
                    }
                }

                return instance;
            }
        }
        public static bool IsExisting => instance != null;

        static T instance;

        virtual protected void Awake()
        {
            CheckInstance();
        }

        protected bool CheckInstance()
        {
            if(instance == null){
                instance = this as T;
                return true;
            }else if(Instance == this) {
                return true;
            }
            Destroy(this);
            return false;
        }
    }
}
