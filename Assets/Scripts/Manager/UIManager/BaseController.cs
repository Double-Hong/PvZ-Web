﻿using UnityEngine;

public class BaseController
{
    private BaseModel model;
    
    public BaseController()
    {
        Debug.Log("Init Controller");
    }

    public void SetModel(BaseModel model)
    {
        this.model = model;
    }

    public T GetModel<T>() where T : BaseModel
    {
        return model as T;
    }

    public void DestroyModel()
    {
        model = null;
    }
}