using UnityEngine;

public class NotDestroyedOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}