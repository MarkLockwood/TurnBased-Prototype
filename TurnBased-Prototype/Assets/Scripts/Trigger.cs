using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] GameObject tutorialUI;

    void OnTriggerEnter(Collider other)
    {
        tutorialUI.SetActive(true);
    }
}