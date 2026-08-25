using UnityEngine;
using UnityEngine.InputSystem;
using UnityRPG.Character.Growth;
using UnityRPG.Character.Player;
using UnityRPG.Character.Stats;
using UnityRPG.Combat;

public sealed class PlayerStatusHUDTester : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGrowth playerGrowth;
    [SerializeField] private PlayerStats playerStats;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f6Key.wasPressedThisFrame)
            DamagePlayer();

        if (Keyboard.current.f7Key.wasPressedThisFrame)
            HealPlayer();

        if (Keyboard.current.f8Key.wasPressedThisFrame)
            AddSmallExperience();

        if (Keyboard.current.f9Key.wasPressedThisFrame)
            AddLargeExperience();

        if (Keyboard.current.f10Key.wasPressedThisFrame)
            LogStatus();
    }

    private void DamagePlayer()
    {
        playerHealth.TakeDamage(new DamageInfo(30f, gameObject));
        LogStatus();
    }

    private void HealPlayer()
    {
        playerHealth.TryHeal(20f);
        LogStatus();
    }

    private void AddSmallExperience()
    {
        playerGrowth.AddExperience(50);
        LogStatus();
    }

    private void AddLargeExperience()
    {
        playerGrowth.AddExperience(500);
        LogStatus();
    }

    private void LogStatus()
    {
        Debug.Log($"[Player Status HUD Test] HP {playerHealth.CurrentHealth}/{playerHealth.MaxHealth} / Lv.{playerGrowth.CurrentLevel} / EXP {playerGrowth.CurrentExperience}/{playerGrowth.RequiredExperience} / MaxHP {playerStats.MaxHealth}");
    }
}