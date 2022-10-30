using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance {get; private set;}

    [Serializable]
    public struct GridVisualTypeMaterial 
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        WhiteSoft,
        Blue,
        BlueSoft,
        Red,
        RedSoft,
        Yellow,
        YellowSoft
    }

    [SerializeField] private Transform gridSystemVisualObjectPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    [SerializeField] private LayerMask obstaclesLayerMask;

    private GridSystemVisualObject[,] gridSystemVisualObjectArray;

    void OnDisable()
    {
        UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        gridSystemVisualObjectArray = new GridSystemVisualObject[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualObjectTransform = Instantiate(gridSystemVisualObjectPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualObjectArray[x, z] = gridSystemVisualObjectTransform.GetComponent<GridSystemVisualObject>();
            }
        }

        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualObjectArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                float unitShoulderHeight = 1.7f;
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition) + Vector3.up * unitShoulderHeight;
                Vector3 testWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition) + Vector3.up * unitShoulderHeight;
                Vector3 aimDir = (testWorldPosition - unitWorldPosition).normalized;

                // Test Whether LOS Is Blocked.
                if (Physics.Raycast(unitWorldPosition, aimDir, Vector3.Distance(unitWorldPosition, testWorldPosition), obstaclesLayerMask)) 
                { 
                    continue; 
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                float unitShoulderHeight = 1.7f;
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition) + Vector3.up * unitShoulderHeight;
                Vector3 testWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition) + Vector3.up * unitShoulderHeight;
                Vector3 aimDir = (testWorldPosition - unitWorldPosition).normalized;

                // Test Whether LOS Is Blocked.
                if (Physics.Raycast(unitWorldPosition, aimDir, Vector3.Distance(unitWorldPosition, testWorldPosition), obstaclesLayerMask)) 
                { 
                    continue; 
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualObjectArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void UpdateGridVisual()
    {
        HideAllGridPositions();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default: 
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                if (selectedUnit != null)
                {
                    if (selectedUnit.GetActionPoints() == 0)
                    {
                        gridVisualType = GridVisualType.WhiteSoft;
                    }
                }
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                if (selectedUnit != null)
                {
                    if (selectedUnit.GetActionPoints() == 0)
                    {
                        gridVisualType = GridVisualType.BlueSoft;
                    }
                }
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                if (selectedUnit != null)
                {
                    if (selectedUnit.GetActionPoints() == 0)
                    {
                        gridVisualType = GridVisualType.YellowSoft;
                    }
                }
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
        }
        if (selectedUnit != null)
        {
            ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could Not Find GridVisualTypeMaterial For GridVisualType " + gridVisualType);
        return null;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
}