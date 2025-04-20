using UnityEngine;

public class BaseModel
{
    public BaseModel()
    {
        Debug.Log("Init Model");
        InitModel();
    }

    protected virtual void InitModel()
    {
        
    }
}