using UnityEngine;

public class BaseView : MonoBehaviour
{
    private string UiName;

    protected virtual void Start()
    {
        Debug.Log("Start");
    }

    public virtual void Show(params object[] args)
    {
        Debug.Log($"Show");
        Init(args);
    }

    protected virtual void Init(params object[] args)
    {
        Debug.Log($"Init {UiName}");
    }

    public virtual void Close()
    {
        Debug.Log($"Close {UiName}");
    }
}