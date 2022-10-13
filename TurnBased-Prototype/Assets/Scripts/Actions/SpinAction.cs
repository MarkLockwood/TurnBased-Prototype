using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        float spinAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);
        totalSpinAmount += spinAmount;
        
        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0f;
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}