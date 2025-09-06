using System;
using System.Collections.Generic;
using UnityEngine;

public class HexCellColorManager : MonoBehaviour
{
    [SerializeField]private HexCellColors hexCellColors;
    
    private Dictionary<HexCellColorEnum, Color> colorDict;

    
    public static HexCellColorManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        BuildDictionary();
    }
    
    public void BuildDictionary()
    {

        colorDict = new Dictionary<HexCellColorEnum, Color>();
        foreach (var item in hexCellColors.hexCellColorItems)
        {
            if (!colorDict.ContainsKey(item.colorEnumType))
            {
                colorDict.Add(item.colorEnumType, item.color);
            }
        }
    }
    
    public Color GetColor(HexCellColorEnum type)
    {

        if (colorDict.TryGetValue(type, out var col))
        {
            return col;
        }

        
        return Color.white;
    }
}