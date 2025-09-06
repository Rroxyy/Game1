using System.Collections.Generic;
using UnityEngine;






[CreateAssetMenu(
    fileName = "HexCellColors",
    menuName = "Akko/HexMap/HexCellColors"
)]
public class HexCellColors : ScriptableObject
{
    public List<HexCellColorItem> hexCellColorItems;
}
