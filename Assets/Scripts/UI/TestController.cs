using UnityEngine;

public class TestController:BaseController
{
    public TestController()
    {
        Debug.Log(111);
        SetModel(new TestModel());
    }
}