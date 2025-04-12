using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace myh
{
    /// <summary>
    /// mono单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        /// <summary>
        /// 单例变量
        /// </summary>
        private static T INSTANCE;

        /// <summary>
        /// 单例提供器
        /// </summary>
        public static T Instance => INSTANCE;

        /// <summary>
        /// 单例初始化
        /// </summary>
        [UsedImplicitly]
        public static void InitSingleton()
        {
            if (INSTANCE != null)
            {
                throw new Exception("单例反复初始化：" + typeof(T).Name);
            }

            GameObject obj = new GameObject(typeof(T).Name);
            obj.AddComponent<T>();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public static void ResetDataAll()
        {
            Manager.ResetDataAll();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public static void DestroyAll()
        {
            Manager.DestroyAll();
        }

        /// <summary>
        /// 单例重置方法，子类需要实现，内容写在这里
        /// </summary>
        protected abstract void ResetData();

        /// <summary>
        /// 单例重置化方法，子类需要实现，内容写在这里
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// 单例销毁方法，子类需要实现，内容写在这里
        /// </summary>
        protected abstract void Destroy();

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnDisable()
        {
        }


        private void Awake()
        {
            INSTANCE = this as T;
            Manager.Add(this);
            OnAwake();
            Init();
        }

        private void Start()
        {
            OnStart();
        }

        /// <summary>
        /// 销毁函数
        /// </summary>
        private void OnDestroy()
        {
            Manager.Remove(this);
            INSTANCE = null;
        }

        void ISingleton.ResetDataI()
        {
            ResetData();
        }

        void ISingleton.DestroyI()
        {
            Destroy();
        }
    }

    static class Manager
    {
        /// <summary>
        /// 实例化列表
        /// </summary>
        private static readonly List<ISingleton> mLIST = new List<ISingleton>();

        public static void Add(ISingleton t)
        {
            if (mLIST.Contains(t))
            {
                throw new Exception("单例反复初始化：" + nameof(t));
            }

            mLIST.Add(t);
        }

        public static void Remove(ISingleton t)
        {
            mLIST.Remove(t);
        }

        public static void ResetDataAll()
        {
            foreach (var t in mLIST)
            {
                t.ResetDataI();
            }
        }

        public static void DestroyAll()
        {
            foreach (var t in mLIST)
            {
                t.DestroyI();
            }

            mLIST.Clear();
        }
    }
}
