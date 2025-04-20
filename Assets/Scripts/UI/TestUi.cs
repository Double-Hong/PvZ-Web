using System.Collections;
using System.Collections.Generic;
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
            num.text = TestModel.inst.num.ToString();
        }));
    }

    protected override void Init()
    {
        base.Init();
        Debug.Log(TestModel.inst.num.ToString());
        num.text = TestModel.inst.num.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
