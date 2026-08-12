
using UnityEngine;

namespace GameData
{
    public class CellData: MonoBehaviour
    {
        private Cell[][] cellList = new Cell[5][];

        private static CellData INSTANCE;

        private CellData()
        {
            for (int i = 0; i < cellList.Length; i++)
            {
                cellList[i] = new Cell[9];
            }
            INSTANCE = this;
        }

        public static CellData GetInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new CellData();
            }

            return INSTANCE;
        }

        public void UpdateCell(Cell cell)
        {
            cellList[cell.row][cell.column] = cell;
        }

    }
}
