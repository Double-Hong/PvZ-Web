using UnityEngine;

public class TestController:BaseController
{
    public static TestController inst
    {
        get
        {
            return ControllerTool.GetController<TestController>();
        }
    }

    public TestController()
    {
        
    }
}