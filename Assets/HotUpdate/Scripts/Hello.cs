using System.Collections;
using UnityEngine;

public class Hello
{
    public static void Run()
    {
        Debug.Log("Hello, World");
        Debug.Log("Hello, myh");
        MainGameManager.InitSingleton();
    }
}