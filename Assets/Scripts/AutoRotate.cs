using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float spinSpeed = 10;
    public float wobbleScale = 30;
    public float wobbleSpeed = 1;

    void Update()
    {
        var newRotation = transform.rotation.eulerAngles;
        newRotation.y = Time.time * spinSpeed % 360f;
        newRotation.x = wobbleScale * Mathf.Sin(Time.time * wobbleSpeed);
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
