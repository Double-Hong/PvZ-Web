using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShovelStateUi : BaseView
{

    public Button CancelBtn;
    // Start is called before the first frame update
    void Start()
    {
        CancelBtn.onClick.AddListener(OnCancelBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCancelBtnClick()
    {
        MainGameManager.GetInstance().ChangeShovelState(false);
        UIManager.Hide("ShovelStateUi");
    }
    
}
