using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpinAction : BaseAction
{
    private Scene scene;

    private float totalSpinAmount;

    private bool TutorialShown = false;
    [SerializeField] private GameObject tutorialUI;

    void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

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
        if (TutorialShown != true)
            {
                if (scene.name == "TutorialScene")
                {
                    TutorialShown = true;
                    tutorialUI.gameObject.SetActive(true);
                }
            }
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

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction 
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}