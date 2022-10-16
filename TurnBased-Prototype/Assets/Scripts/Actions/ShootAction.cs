using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    private enum State
    {
        Aiming,
        Shooting,
        CoolOff,
    }

    [SerializeField] private LayerMask obstacleLayerMask;

    private State state;
    private float stateTimer;
    private int maxShootDistance = 7;
    private Unit targetUnit;

    private bool canShootBullet;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
            Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
            break;
            case State.Shooting:
            if (canShootBullet)
            {
                Shoot();
                canShootBullet = false;
            }
            break;
            case State.CoolOff:
            break;
        }

        if (stateTimer <= 0f) 
            {
                NextState();
            }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
            state = State.Shooting;
            float shootingStateTime = 0.1f;
            stateTimer = shootingStateTime;
            break;
            case State.Shooting:
            state = State.CoolOff;
            float coolOffStateTime = 0.5f;
            stateTimer = coolOffStateTime;
            break;
            case State.CoolOff:
            ActionComplete();
            break;
        }

        //Debug.Log(state);
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs {targetUnit = targetUnit, shootingUnit = unit});
        OnShoot?.Invoke(this, new OnShootEventArgs {targetUnit = targetUnit, shootingUnit = unit});
        targetUnit.Damage(40);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // There Is No Unit On That Grid Space.
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both Units Are On The Same Team.
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, shootDir, Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()), obstacleLayerMask))
                {
                    //Blocked By Obstacle.
                    continue;
                }

                if (unit.GetActionPoints() == 0)
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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        //Debug.Log("Aiming");
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }
    
    public override int GetActionPointsCost()
    {
        return 1;
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction 
        {
            gridPosition = gridPosition,
            //actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
            actionValue = 100 + Mathf.RoundToInt(targetUnit.GetAttackScore())
        };
    }
}