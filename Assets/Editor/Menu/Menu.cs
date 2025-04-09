using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using GameData;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace Assets.Scripts
{

    class Node
    {
        public string name;

        public string display;

        public Node(string name, string display)
        {
            this.name = name;
            this.display = display;
        }
    }
    
    class Menu
    {

        [MenuItem("Myh/Test", false)]
        public static void Test()
        {
            
            PlantInfoConfig config = ConfigManager.GetConfigById<PlantInfoConfig>(4);
            if (config != null)
            {
                Debug.Log($"植物名称: {config.name}，冷却时间: {config.cooldown}");
            }
        }

        static int Compare(string a, string b)
        {
            if (a.Contains("/")) return -1;  // "myh" 优先级最高，排在最前面
            if (b.Contains("/")) return 1;
            return string.Compare(a, b, StringComparison.Ordinal); // 默认按字母排序
        }

        private static void MySort(Func<string,string,int> compare = null)
        {
            List<Node> list = new List<Node>();
            Node node1 = new Node("myh", "Myh");
            Node node2 = new Node("wyx/", "Wyx");
            Node node3= new Node("zwj/", "Zwj");
            Node node4 = new Node("fzf/", "Fzf");
            list.Add(node1);
            list.Add(node2);
            list.Add(node3);
            list.Add(node4);

            if (compare != null)
            {
                list.Sort((a,b) => compare(a.name,b.name));
            }

            foreach (Node node in list)
            {
                Debug.Log(node.name);
            }
        }
        
        [MenuItem("Myh/关卡/重新加载关卡", false)]
        public static void LoadLevel()
        {
            MainGameManager.GetInstance().OnLevelEnter();
        }
        
        private static void Add(ref int num)
        {
            num++;
        }

        [MenuItem("MainScene/Main",false)]
        public static void MainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }
        
        /// <summary>
        /// 加阳光
        /// </summary>
        [MenuItem("Myh/阳光/加1000阳光")]
        public static void AddSun()
        {
            SunManager.GetInstance().GetSun(1000);
        }

        /// <summary>
        /// 减阳光
        /// </summary>
        [MenuItem("Myh/阳光/减1000阳光")]
        public static void SubtractSun()
        {
            SunManager.GetInstance().UseSun(1000);
        }

        [MenuItem("Myh/阳光/天空停止阳光")]
        public static void StopSunFromSky()
        {
            SunManager.GetInstance().StopSunFromSky();
        }

        [MenuItem("Myh/阳光/收集阳光")]
        public static void GetAllSun()
        {
            Sun[] suns = SunManager.GetInstance().sunList.GetComponentsInChildren<Sun>();
            foreach (Sun sun in suns)
            {
                sun.SetState(SunState.Clicked);
            }
            SunManager.GetInstance().GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/GetSun"));
        }

        [MenuItem("Myh/摄像机/左右")]
        public static void MoveCamera()
        {
            MainGameManager.GetInstance().PreparationMoveCamera(() => Debug.Log("1"));
        }
    }
}