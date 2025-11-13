using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    
    public static PlayerUIManager instance;

    [HideInInspector] public PlayerUIHudManager playerUIHUdManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else 
        {
            Destroy(gameObject);
        }

        playerUIHUdManager = GetComponentInChildren<PlayerUIHudManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
