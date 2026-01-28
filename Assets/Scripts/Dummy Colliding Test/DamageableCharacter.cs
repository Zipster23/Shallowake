using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour
{
    
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material hitMaterial;
    private Renderer meshRenderer;

    private void Awake()
    {
        currentHealth = maxHealth;
        meshRenderer = GetComponent<Renderer>();

        if(meshRenderer != null)
        {
            normalMaterial = meshRenderer.material;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        Debug.Log($"{gameObject.name} took {damageAmount} damage! Health: {currentHealth}/{maxHealth}");

        if(meshRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if(meshRenderer != null)
        {
            meshRenderer.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        if(meshRenderer != null)
        {
            meshRenderer.material.color = Color.white;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"{gameObject.name} healed {healAmount}! Health: {currentHealth}/{maxHealth}");
    }

}
