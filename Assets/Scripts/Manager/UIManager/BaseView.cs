using UnityEngine;

public class BaseView : MonoBehaviour
{
    private string UiName;

    protected virtual void Start()
    {
        Debug.Log("Start");
    }

    public virtual void Show()
    {
        Debug.Log($"Show");
        Init();
    }

    protected virtual void Init()
    {
        Debug.Log($"Init {UiName}");
    }

    protected virtual void Close()
    {
        Debug.Log($"Close {UiName}");
    }
}