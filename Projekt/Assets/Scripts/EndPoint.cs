﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        other.GetComponentInParent<BomberAgent>().targetFound = true;
    }
}
