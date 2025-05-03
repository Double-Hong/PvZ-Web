using System;
using System.Collections;
using UnityEngine;

namespace myh
{
    /// <summary>
    /// 延迟调用
    /// </summary>
    public class DelayCall : MonoBehaviour
    {
        private static DelayCall _instance;

        private static DelayCall Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("DelayCall");
                    _instance = go.AddComponent<DelayCall>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 延迟调用一个方法
        /// </summary>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="callback">要执行的方法</param>
        public static void After(float delay, Action callback)
        {
            Instance.StartCoroutine(Instance.DelayCoroutine(delay, callback));
        }

        private IEnumerator DelayCoroutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}
