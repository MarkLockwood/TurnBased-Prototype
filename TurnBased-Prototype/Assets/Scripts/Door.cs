using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    public static event EventHandler OnAnyDoorOpened;
    public event EventHandler OnDoorOpened;

    private Scene scene;

    [SerializeField] private bool isOpen;
    
    private bool TutorialShown = false;
    [SerializeField] private GameObject tutorialUI;

    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;

    private AudioSource audioSource;
    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip doorClose;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        scene = SceneManager.GetActiveScene();

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void Update()
    {
        if (!isActive) { return; }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = false;
            onInteractionComplete();
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        timer = .5f;
        
        if (isOpen)
        {
            CloseDoor();
            PlayCloseDoorAudio();
            if (TutorialShown != true)
            {
                if (scene.name == "TutorialScene")
                {
                    TutorialShown = true;
                    tutorialUI.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            OpenDoor();
            PlayOpenDoorAudio();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);

        OnDoorOpened?.Invoke(this, EventArgs.Empty);
        OnAnyDoorOpened?.Invoke(this, EventArgs.Empty);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }

    private void PlayOpenDoorAudio()
    {
        audioSource.clip = doorOpen;
        audioSource.PlayOneShot(doorOpen);
    }

    private void PlayCloseDoorAudio()
    {
        audioSource.clip = doorClose;
        audioSource.PlayOneShot(doorClose);
    }
}