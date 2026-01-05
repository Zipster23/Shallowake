using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAIManager : MonoBehaviour
{
    
    public static WorldAIManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
