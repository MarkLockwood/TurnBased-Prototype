using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private int maxThrowDistance = 7;
    [SerializeField] private LayerMask obstacleLayerMask;

    void Update()
    {
        if (!isActive) { return; }
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }
                if (ObstacleInTheWay(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
        
        //Debug.Log("GrenadeAction");
        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

    private bool ObstacleInTheWay(GridPosition unitGridPosition, GridPosition targetGridPosition)
    {
        // Get the unit world position
        var unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
        // Get the target world position
        var targetWorldPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        // Get the direction to the target
        var directionToTarget = (targetWorldPosition - unitWorldPosition).normalized;
        // Get the distance to the target
        var distanceToTarget = Vector3.Distance(unitWorldPosition, targetWorldPosition);
        // Create the ray - with height offset
        var offset = Vector3.up * 1f; // <- magic number. sorry
        var ray = new Ray(unitWorldPosition + offset, directionToTarget);
        // Check if there is an obstacle in the way and return
        return Physics.Raycast(ray, distanceToTarget, obstacleLayerMask);
    }
}