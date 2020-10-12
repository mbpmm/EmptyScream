﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalShooter : MonoBehaviour {
    public Texture2D decal;
    public LayerMask hitMask;
    public Color color;
    [Range(0f,5f)]
    public float size;
    void FixedUpdate() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, hitMask, QueryTriggerInteraction.Ignore)) {
            SkinnedMeshRenderer r = hit.collider.transform.root.GetComponentInChildren<SkinnedMeshRenderer>();
            if (r != null) {
                PaintDecal.instance.RenderDecal(r, decal, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up), color, size);
            }
        }
    }
}
