﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float destroyDelay;

    private void Update()
    {
        destroyDelay -= Time.deltaTime;
        if (destroyDelay <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}