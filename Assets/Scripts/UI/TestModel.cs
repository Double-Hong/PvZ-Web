using UnityEngine;

public class TestModel:BaseModel
{
    public static TestModel inst => 
        ControllerTool.GetController<TestController>().GetModel<TestModel>();

    public int num;

    protected override void InitModel()
    {
        base.InitModel();
        Debug.Log("Sub init");
        num = 3;
    }
}