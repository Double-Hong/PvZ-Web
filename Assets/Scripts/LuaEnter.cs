// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;
// using XLua;
//
// public class LuaEnter : MonoBehaviour
// {
//     LuaEnv luaEnv;
//
//     [CSharpCallLua]
//     public delegate double LuaMax(double a, double b);
//     void Start()
//     {
//         luaEnv = new LuaEnv();
//         //Lua调用C# Log方法 输出日志
//         luaEnv.DoString("CS.UnityEngine.Debug.Log('hello world')");
//         //C#调用Lua系统函数 返回最大值
//         var max = luaEnv.Global.GetInPath<LuaMax>("math.max");
//         Debug.Log("max:" + max(32, 12));
//         luaEnv.AddLoader(MyLoader);
//
//         luaEnv.DoString("require 'TestLua'");
//
//         int a = luaEnv.Global.Get<int>("b");
//         Debug.Log(a);
//         Person p = luaEnv.Global.Get<Person>("person");
//         Debug.Log($"{p.name}/{p.age}");
//     }
//     //自定义Loader方法
//     private byte[] MyLoader(ref string filePath)
//     {
//         print("调用自定义Loader");
//         //找到你想要查找的路径
//         string path = Application.dataPath + "/Resources/LuaProjects/Test/"  + filePath + ".lua";
//         //通过UTF-8编码读取文件中的内容
//         return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(path));
//     }
//     void OnDestroy()
//     {      
//         luaEnv.Dispose();//释放lua
//     }
//
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
//     
//     
// }
//
// public class Person
// {
//     public string name;
//
//     public int age;
// }
