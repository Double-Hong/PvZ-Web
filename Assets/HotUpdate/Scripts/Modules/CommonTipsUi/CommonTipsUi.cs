using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommonTipsUi : BaseView
{
    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Awake()
    {
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(text.font);
    }

    protected override void Init(params object[] args)
    {
        base.Init(args);
        Debug.Log($"text is null -->{text == null}");
        Debug.Log(text.name);
        if (args[0] != null) text.text = args[0].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseSelf()
    {
        UIManager.Close("CommonTipsUi");
    }
}
