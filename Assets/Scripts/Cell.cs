using UnityEngine;

public class Cell : MonoBehaviour
{
    public Plant currentPlant;

    public int row;
    
    public int column;
    
    void Start()
    {
        InitializeRowAndColumn();
    }
    

    /// <summary>
    /// 单元格被点击时
    /// </summary>
    private void OnMouseDown()
    {
        if (currentPlant != null)
        {
            return;
        }

        HandManager.GetInstance().OnCellMouseDown(this);
    }

    /// <summary>
    /// 初始化行和列
    /// </summary>
    private void InitializeRowAndColumn()
    {
        float x = transform.localPosition.x + 0.45f;
        
        float y = transform.localPosition.y + 0.41f;
        //让行数=（x/0.11）的四舍五入
        column = Mathf.RoundToInt(x / 0.11f);
        //让列数=（y/0.2）的四舍五入
        row = Mathf.RoundToInt(y / 0.2f);
    }
}