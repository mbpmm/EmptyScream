﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour
{
    public delegate void OnPlayerStatusAction(float amount);
    public delegate void OnPlayerStatusAction2();
    public static OnPlayerStatusAction OnPlayerChangeHP;
    public static OnPlayerStatusAction OnPlayerChangeSanity;
    public static OnPlayerStatusAction OnPlayerChangeSanityTier;
    public static OnPlayerStatusAction2 OnPlayerHurt;
    public static OnPlayerStatusAction2 OnPlayerAffectedBySanity;
   // public static OnPlayerStatusAction OnPlayerChangeSanityStatus;

    [System.Serializable]
    public struct SanityTierChanges
    {
        public float maxHealth;
        public float damageMultiplier;
        public float speedMultiplier;
    }

    [Header("Player Config")]
    public float maxHealth;
    public float maxSanity;
    public float walkSpeed;
    public float runSpeed;
    public float crouchMovSpeed;
    public Vector2[] sanityTierRanges;
    public SanityTierChanges[] sanityTierChanges;
    public int sanityCurrentTier;
    private TraumaInducer screenShake;
    

    [Header("Player Current State")]
    public float health;
    public float sanity;
    public float damageMultiplier;
    public float speedMultiplier;

    //public float damageSpeed;
    public bool isBeingDamaged;
    private FirstPersonController fpsController;
    private int lastSanityIndex;

    //[Header("Components Assigment")]
    //Player Inventory;

    // Start is called before the first frame update
    void Start()
    {
        screenShake = GetComponent<TraumaInducer>();
        fpsController = GetComponent<FirstPersonController>();
        fpsController.m_WalkSpeed = walkSpeed;
        fpsController.m_RunSpeed = runSpeed;
        fpsController.crouchMovSpeed = crouchMovSpeed;
        lastSanityIndex = -1;

        health = maxHealth;
        sanity = maxSanity;
        if (OnPlayerChangeHP != null)
        {
            OnPlayerChangeHP(health);
        }

        if (OnPlayerChangeSanity != null)
        {
            OnPlayerChangeSanity(sanity);
        }

        ApplyNewSanityStatus(0);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(isBeingDamaged)
        {
            CauseDamage(damage);
        }*/
    }

    public void CauseDamage(float amount)
    {
        health -= amount;

        if(health <= 0)
        {
            health = 0;
        }

        if(OnPlayerChangeHP != null)
        {
            OnPlayerChangeHP(health);
        }

        if(OnPlayerHurt != null)
        {
            OnPlayerHurt();
        }

        StartCoroutine(screenShake.Shake());
    }

    public void ChangeSanityValue(float amount)
    {
        sanity += amount;

        for (int i = 0; i < sanityTierRanges.Length; i++)
        {
            if (sanity <= sanityTierRanges[i].y && sanity >= sanityTierRanges[i].x)
            {
                ApplyNewSanityStatus(i);
                i = sanityTierRanges.Length;
            }
        }

        if(sanity <= 0)
        {
            sanity = 0;
        }

        if(OnPlayerChangeSanity != null)
        {
            OnPlayerChangeSanity(sanity);
        }

        if(OnPlayerAffectedBySanity != null)
        {
            OnPlayerAffectedBySanity();
        }

        /*if (sanity <= sanityTierRanges[0].y)
        {
           // ApplyNewStatus(0);
        }
        if (sanity <= sanityTierRanges[1].y)
        {
            //ApplyNewStatus(1);
        }
        if (sanity <= sanityTierRanges[2].y)
        {
           // ApplyNewStatus(2);
           // blood.ActivateMask();
        }*/
    }

    private void ApplyNewSanityStatus(int index)
    {
        //sanityTierChanges[0];
        if(index != lastSanityIndex)
        {
            maxHealth = sanityTierChanges[index].maxHealth;
            health = maxHealth;
            damageMultiplier = sanityTierChanges[index].damageMultiplier;
            speedMultiplier = sanityTierChanges[index].speedMultiplier;
            fpsController.m_WalkSpeed = walkSpeed * sanityTierChanges[index].speedMultiplier;
            fpsController.m_RunSpeed = runSpeed * sanityTierChanges[index].speedMultiplier;
            fpsController.crouchMovSpeed = crouchMovSpeed * sanityTierChanges[index].speedMultiplier;
            lastSanityIndex = index;

            if (OnPlayerChangeSanityTier != null)
            {
                OnPlayerChangeSanityTier(index);
            }

            if (OnPlayerChangeHP != null)
            {
                OnPlayerChangeHP(health);
            }

            Debug.Log("Applied Sanity Tier : " + (index));
            sanityCurrentTier = index + 1;
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "enemy")
        {
            DummyEnemyTest enemy = other.gameObject.GetComponent<DummyEnemyTest>();

            //damage = enemy.damage;
            isBeingDamaged = true;

           // Debug.Log("gg ameo");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            isBeingDamaged = false;
        }
    }
}
