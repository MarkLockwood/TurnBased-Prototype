using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwordAction : BaseAction
{
    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit
    }

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    public static event EventHandler OnAnySwordHit;

    private int maxSwordDistance = 1;

    private State state;
    private float stateTimer;

    private Unit targetUnit;

    private AudioSource audioSource;
    [SerializeField] private AudioClip swordSwing;

    private Scene scene;
    private bool TutorialShown = false;
    [SerializeField] private GameObject tutorialUI;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        scene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (!isActive) { return; }

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
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
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                PlayAudio();
                OnAnySwordHit.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                if (TutorialShown != true)
                {
                    if (scene.name == "TutorialScene")
                    {
                        TutorialShown = true;
                        tutorialUI.gameObject.SetActive(true);
                    }
                }
                ActionComplete();
                break;
        }

        //Debug.Log(state);
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction {gridPosition = gridPosition, actionValue = 200};
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
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

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }

    private void PlayAudio()
    {
        audioSource.clip = swordSwing;
        audioSource.PlayOneShot(swordSwing);
    }
}
