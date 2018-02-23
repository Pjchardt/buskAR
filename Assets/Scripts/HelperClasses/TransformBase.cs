using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformBase : MonoBehaviour
{
    float moveScale = .1f;

    public void Rotate(int i)
    {
        transform.Rotate(0f, i * 15f, 0f);
    }

    public void TranslateX(int i)
    {
        transform.Translate(Vector3.forward * i * moveScale); //swap forward and right because I fucked up and put the rail for the ball down the x axis and not along the forward axis...
    }

    public void TranslateY(int i)
    {
        transform.Translate(Vector3.up * i * moveScale);
    }

    public void TranslateZ(int i)
    {
        transform.Translate(Vector3.right * i * moveScale);
    }
}
