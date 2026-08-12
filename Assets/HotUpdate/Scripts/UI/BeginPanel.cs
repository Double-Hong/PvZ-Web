using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginPanel : MonoBehaviour
{
    void Start()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UserPanel");
        Instantiate(prefab,MainGameManager.GetInstance().GameCanvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GoToGame()
    {
        Time.timeScale = 1f;
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
