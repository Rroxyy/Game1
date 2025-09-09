using System;
using UnityEngine;

public class HexCellCoordShowManager : MonoBehaviour
{
    [Header("Base")]
    [SerializeField]private GameObject showIdPrefab;
    [SerializeField]private RectTransform instanceTransform;

    [SerializeField] private bool showCubeCoords=false;

    
    public static HexCellCoordShowManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public HexCellCoordUI GetHexCellCoordUI(HexCell cell)
    {
        var hexCellCoordUI = Instantiate(showIdPrefab, instanceTransform).GetComponent<HexCellCoordUI>();
        hexCellCoordUI.SetUp(cell,showCubeCoords);
        return hexCellCoordUI;
    }
}