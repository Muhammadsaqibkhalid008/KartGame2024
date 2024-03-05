using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartRotator : MonoBehaviour
{
    [SerializeField] float kartRotationSpeed;
    [SerializeField] bool leftToRight;
    private float y = 0f;

    private void Update()
    {
        if (leftToRight) y += kartRotationSpeed * Time.deltaTime;
        else y -= kartRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, y, 0f);
    }
}
