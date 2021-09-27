using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform lookat = null;
    public Transform moveto = null;
    public float lookspeed = 40;
    public float movespeed = 4;

    private void LateUpdate()
    {
        if (lookat != null)
        {
            Quaternion rotTarget = Quaternion.LookRotation(lookat.position - this.transform.position);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, lookspeed * Time.deltaTime);
        }

        if (moveto != null)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, moveto.position, movespeed * Time.deltaTime);
        }
    }
}
