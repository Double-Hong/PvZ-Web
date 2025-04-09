using System;
using System.Collections.Generic;
using GameData;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    private static HandManager INSTANCE;
    
    private HandManager()
    {
        INSTANCE = this;
    }

    public static HandManager GetInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new HandManager();
        }

        return INSTANCE;
    }

    public List<Plant> plantPrefabList;

    /// <summary>
    /// 当前手上的植物
    /// </summary>
    [SerializeField] 
    private Plant currentPlant;

    /// <summary>
    /// 当前手上植物的阳光需求
    /// </summary>
    [SerializeField]
    private int currentSunNeed;

    /// <summary>
    /// 当前植物种植成功时的回调
    /// </summary>
    private Action onPlantSuccess;
    
    private void Update()
    {
        if (currentPlant != null)
        {
            FollowCursor();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentPlant != null)
            {
                Destroy(currentPlant.gameObject);
                currentPlant = null;
            }
        }
    }


    /// <summary>
    /// 将植物添加到Hand中
    /// </summary>
    /// <param name="plantType">植物类型</param>
    /// <param name="sunNeed">该植物所需阳光</param>
    /// <param name="onPlantSuccess">当该植物被种植时的回调</param>
    public void AddPlant(PlantType plantType, int sunNeed, Action onPlantSuccess)
    {
        if (currentPlant != null)
        {
            Debug.Log("手上已经有植物了");
            return;
        }

        Plant plantPrefab = GetPlantPrefab(plantType);
        if (plantPrefab == null)
        {
            Debug.Log("植物类型错误");
            return;
        }
        
        currentPlant = Instantiate(plantPrefab,MainGameManager.GetInstance().root.transform);
        currentPlant.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        currentPlant.GetComponent<BoxCollider2D>().enabled = false;
        currentSunNeed = sunNeed;
        this.onPlantSuccess = onPlantSuccess;
    }

    /// <summary>
    /// 获取植物预制体
    /// </summary>
    /// <param name="plantType">植物类型</param>
    /// <returns>对应Prefab</returns>
    private Plant GetPlantPrefab(PlantType plantType)
    {
        foreach (Plant plant in plantPrefabList)
        {
            if (plant.plantType == plantType)
            {
                return plant;
            }
        }

        return null;
    }

    /// <summary>
    /// 使植物跟随鼠标
    /// </summary>
    private void FollowCursor()
    {
        if (currentPlant == null)
        {
            return;
        }
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        currentPlant.transform.position = mouseWorldPosition;
    }

    /// <summary>
    /// 当单元格被点击时
    /// </summary>
    /// <param name="cell">被点击的单元格</param>
    public void OnCellMouseDown(Cell cell)
    {
        if (currentPlant == null) return;
        currentPlant.transform.position = cell.transform.position;
        currentPlant.TurnToEnable();
        SunManager.GetInstance().UseSun(currentSunNeed);
        onPlantSuccess();
        cell.currentPlant = currentPlant;
        currentPlant.row = cell.row;
        currentPlant.column = cell.column;
        CellData.GetInstance().UpdateCell(cell);
        currentPlant.GetComponent<BoxCollider2D>().enabled = true;
        currentPlant = null;
        MainGameManager.GetInstance().audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/plant"));
    }
    
    
}