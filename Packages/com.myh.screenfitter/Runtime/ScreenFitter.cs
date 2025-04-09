using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

namespace myh
{
    public class ScreenFitter : MonoBehaviour
    {
        private void Start()
        {
            FitScreen();
        }

        /// <summary>
        /// 屏幕适配
        /// </summary>
        private void FitScreen()
        {
            CanvasScaler cs = transform.root.GetComponentInChildren<CanvasScaler>();
#if UNITY_EDITOR
            Rect safeArea = Screen.safeArea;
            float py = (float)safeArea.yMin / (float)safeArea.height;
            transform.GetComponent<RectTransform>().anchorMin = new Vector2(
                (float)safeArea.left / (float)safeArea.width, -(float)safeArea.top / (float)safeArea.height);
            cs.referenceResolution = new Vector2(cs.referenceResolution.x, cs.referenceResolution.y * (1.0f + py));
#elif minigame
            var info = WX.GetWindowInfo();
            float py = (float)info.safeArea.top / (float)info.windowHeight;
            transform.GetComponent<RectTransform>().anchorMin =
                new Vector2((float)info.safeArea.left / (float)info.windowWidth,
                    -(float)info.safeArea.top / (float)info.windowHeight);
            cs.referenceResolution = new Vector2(cs.referenceResolution.x, cs.referenceResolution.y * (1.0f + py));
#endif
        }
    }
}