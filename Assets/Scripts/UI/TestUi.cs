using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUi : BaseView
{

    public TextMeshProUGUI num;

    public Button btn;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        btn.onClick.AddListener((() =>
        {
            // UIManager.Close("TestUi");
            num.transform.position = new Vector3(num.transform.position.x - 1, num.transform.position.y);
        }));
    }

    private void Update()
    {
        // Debug.Log($"local ---> {btn.transform.localPosition}");
        // Debug.Log($"position ---> {btn.transform.position}");
        // RectTransform rect = btn.GetComponent<RectTransform>();
        // Debug.Log($"rect local ---> {rect.localPosition}");
        // Debug.Log($"rect position ---> {rect.position}");
    }

    protected override void Init(params object[] args)
    {
        base.Init();
        TestController.inst.SetModel(new TestModel());
        Debug.Log(TestModel.inst.num.ToString());
        num.text = TestModel.inst.num.ToString();
    }

    public override void Close()
    {
        base.Close();
        TestController.inst.DestroyModel();
    }
}
