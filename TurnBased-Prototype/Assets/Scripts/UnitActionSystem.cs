using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance {get; private set;}

    [SerializeField] private LayerMask unitLayerMask;

    [SerializeField] private Unit selectedUnit;

    private BaseAction selectedAction;

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    private bool isBusy;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        SetSelectedUnit(selectedUnit);
    }
    
    void Update() 
    {
        if (isBusy) { return; }
        if (!TurnSystem.Instance.IsPlayerTurn()) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        // Try Getting Unit That Was Clicked.
        if (TryGetClickedUnit(out Unit clickedUnit))
        {
            // Clicked On A Player Unit.
            if (TryHandlePlayerUnitClicked(clickedUnit)) { return; }
            // Clicked On An Enemy Unit.
            if (TryHandleEnemyUnitClicked(clickedUnit)) { return; }
        }
        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorldPosition.GetPosition());
        HandleSelectedAction(mouseGridPosition);
    }

    private void HandleSelectedAction(GridPosition gridPosition)
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            if (!selectedAction.IsValidActionGridPosition(gridPosition)) { return; }
            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) { return; }

            SetBusy();
            selectedAction.TakeAction(gridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryGetClickedUnit(out Unit clickedUnit)
    {
        clickedUnit = default;
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out clickedUnit))
                {
                    // A Unit Was Clicked On.
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryHandlePlayerUnitClicked(Unit clickedUnit)
    {
        // Unit Is Already Selected.
        if (clickedUnit == selectedUnit) { return false; }
        // Unit Is An Enemy.
        if (clickedUnit.IsEnemy()) { return false; }
        SetSelectedUnit(clickedUnit);
        return true;
    }

    private bool TryHandleEnemyUnitClicked(Unit clickedUnit)
    {
        // Clicked On A Player Unit.
        if (!clickedUnit.IsEnemy()) { return false; }
        var enemyGridPosition = clickedUnit.GetGridPosition();
        HandleSelectedAction(enemyGridPosition);
        return true;
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
