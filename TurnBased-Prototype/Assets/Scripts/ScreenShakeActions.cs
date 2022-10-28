using System;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip grenadeExplosion;

    void OnDisable()
    {
        ShootAction.OnAnyShoot -= ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded -= GrenadeProjectile_OnAnyGrenadeExploded;
        SwordAction.OnAnySwordHit -= SwordAction_OnAnySwordHit;
    }

    void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
        audioSource = GetComponent<AudioSource>();
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
        PlayAudio();
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(2f);
    }

    public void PlayAudio()
    {
        audioSource.clip = grenadeExplosion;
        audioSource.PlayOneShot(grenadeExplosion);
    }
}