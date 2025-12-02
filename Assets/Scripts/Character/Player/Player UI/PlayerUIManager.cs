using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    
    public static PlayerUIManager instance;
    public PlayerUIHudManager playerUIHudManager;

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
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
