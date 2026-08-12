using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TestUi : BaseView
{
    public Button btn;
    public PlayableDirector director;

    void Awake()
    {
        // btn = transform.Find("Button")?.GetComponent<Button>();
        // director = transform.GetComponentInChildren<PlayableDirector>(true);
    }

    protected override void Start()
    {
        base.Start();
        if (btn != null)
        {
            btn.onClick.AddListener(OnPlayBtnClick);
        }
    }

    protected override void Init(params object[] args)
    {
        base.Init(args);
        if (director != null)
        {
            director.Stop();
            director.time = 0;
            director.Evaluate();
        }
    }

    private void OnPlayBtnClick()
    {
        if (director == null)
        {
            Debug.LogError("TestUi 未找到 PlayableDirector");
            return;
        }

        director.time = 0;
        director.Play();
    }

    public override void Close()
    {
        if (btn != null)
        {
            btn.onClick.RemoveListener(OnPlayBtnClick);
        }
        base.Close();
    }
}
