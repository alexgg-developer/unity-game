using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RiceAnimationBehaviour: MonoBehaviour
{

    void Start()
    {

    }

    float rotationsPerMinute = 40.0f;
    void Update()
    {
        transform.Rotate(0, 0, -6.0f * rotationsPerMinute * Time.deltaTime);
    }
}
