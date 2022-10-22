using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] AudioClip[] hurtSounds;

    private AudioSource audioSource;

    [SerializeField] private int health = 100;
    private int healthMax;

    void Awake()
    {
        healthMax = health;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Damage(int damageAmount)
    {
        PlayHurtSound();
        health -= damageAmount;

        if (health < 0)
        {
            health = 0;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (health == 0)
        {
            Die();
        }

        //Debug.Log(health);
    }

    private void Die()
    {
        OnDead.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }

    public float AttackScore()
    {
        float unitPerHealthPoint = 100/healthMax;  // the higher the health, the lower this score will be
        return (healthMax-health) * unitPerHealthPoint + unitPerHealthPoint;
    }

    private void PlayHurtSound()
    {
        int randomHurtSound = UnityEngine.Random.Range(1, hurtSounds.Length);
        audioSource.clip = hurtSounds[randomHurtSound];
        audioSource.PlayOneShot(audioSource.clip);

        hurtSounds[randomHurtSound] = hurtSounds[0];
        hurtSounds[0] = audioSource.clip;
    }
}