using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailActivator : MonoBehaviour
{
    public TrailRenderer[] m_TrailRenderers;

    private void OnEnable()
    {
        foreach (TrailRenderer renderer in m_TrailRenderers)
        {
            renderer.emitting = true;
        }
    }
    private void OnDisable()
    {
        foreach (TrailRenderer renderer in m_TrailRenderers)
        {
            renderer.emitting = false;
        }
    }
}
