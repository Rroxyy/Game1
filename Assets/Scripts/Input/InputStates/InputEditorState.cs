using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class InputEditorState : InputState
{
    [FormerlySerializedAs("changeRadius")]
    [Header("Base")] 
    [SerializeField, Range(0, 10)] private int radius = 1;
    
    [Header("Color Mode")]
    [SerializeField]private HexCellColorEnum hexCellColorEnum = HexCellColorEnum.White;
    
    [Header("Height Mode")]
    [SerializeField]private uint height = 2;
    
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var cell = InputManager.instance.GetMouseRayCell();
            if (cell == null) return;
            List<HexCell> neighbors = new List<HexCell>();
            // List<HexCellDirection> directions = new List<HexCellDirection>();
            
            neighbors.Add(cell);
            HexCellMapManager.instance.GetCellNeighbors(cell, ref neighbors,(uint)radius);

            foreach (var it in neighbors)
            {
                ChangeHexCellColor(it);
                ChangeHexCellHeight(it);
            }
            
            
        }
        
    }

    private void ChangeHexCellHeight(HexCell cell)
    {
        if (cell == null) return;
        cell.SetHeight(height);
    }

    private void ChangeHexCellColor(HexCell cell)
    {
        if (cell == null) return;
        cell.SetColor(HexCellColorManager.instance.GetColor(hexCellColorEnum));
    }
}