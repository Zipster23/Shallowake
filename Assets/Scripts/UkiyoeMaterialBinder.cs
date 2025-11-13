using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UkiyoeMaterialBinder : MonoBehaviour
{
    public Material targetMaterial;
    public Transform centerTransform; // optional, if left null uses object's transform
    public Light mainLight; // optional, if left null uses a directional light at (0,1,0)
    public bool useObjectOrigin = true;

    void Update()
    {
        if (targetMaterial == null) return;

        Vector3 centerWorld = (centerTransform != null) ? centerTransform.position : transform.position;
        targetMaterial.SetVector("_CenterWorld", new Vector4(centerWorld.x, centerWorld.y, centerWorld.z, 1));
        targetMaterial.SetFloat("_UseOriginCenter", useObjectOrigin ? 1f : 0f);

        if (mainLight != null)
        {
            Vector3 lp = mainLight.transform.position;
            targetMaterial.SetVector("_LightPosWorld", new Vector4(lp.x, lp.y, lp.z, 1));
            targetMaterial.SetFloat("_LightStrength", (mainLight.type == LightType.Directional) ? 1.0f : mainLight.intensity);
        }
        else
        {
            // safe default
            Vector3 defaultLight = transform.position + Vector3.up * 1.0f;
            targetMaterial.SetVector("_LightPosWorld", new Vector4(defaultLight.x, defaultLight.y, defaultLight.z, 1));
        }
    }
}
