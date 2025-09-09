using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HexCellCoordUI : MonoBehaviour
{
    [Header("Base")]
    [SerializeField]private TextMeshProUGUI textMesh;
    [SerializeField]private RectTransform rectTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(HexCell cell,bool showCubeCoordinates)
    {
        gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
        HexCellCoords id = cell.hexCellCoords;
        if (showCubeCoordinates)
        {
            var _id = HexCellMetrics.OffsetToCube(id);
            textMesh.text = _id.ToString();
        }
        else
        {
            textMesh.text=id.ToString();
        }

        var pos= cell.positionWS;
        pos.y += 0.1f;
        rectTransform.position = pos;

    }
}
