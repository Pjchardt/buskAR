using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject
{
    public GameObject obj;

    float elevationAngle;
    float inclinationLevel;

    public ParticleObject(Material m)
    {
        elevationAngle = Random.Range(-Mathf.PI, Mathf.PI);
        inclinationLevel = Random.Range(-Mathf.PI, Mathf.PI);
        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = Vector3.one * .01f;
        obj.GetComponent<Renderer>().material = m;
        obj.layer = LayerMask.NameToLayer("AR");
    }

    public void UpdateObject(Vector3 origin, float delta, float radius, float scale, float yRotationScale)
    { 
        Vector3 v = Vector3.zero;
        float radiusDelta = radius + (Mathf.Sin(delta) * (radius / 5f));
        v.x = radiusDelta * Mathf.Sin(elevationAngle * delta) * Mathf.Cos(inclinationLevel * delta);
        v.y = (radiusDelta * yRotationScale) * Mathf.Sin(elevationAngle * delta) * Mathf.Sin(inclinationLevel * delta);
        v.z = radiusDelta * Mathf.Cos(elevationAngle * delta);

        obj.transform.position = origin + DataObject.Instance.ARObjectToTrack.transform.rotation * v;
        obj.transform.localScale = Vector3.one * scale;
    }
}

public class ParticleTest : MonoBehaviour
{
    public int MaxObjects;
    ParticleObject[] pObjects;

    float timeOfSimulation;

    public Material CubeMaterial;

    private void Start()
    {
        pObjects = new ParticleObject[MaxObjects];

        for (int i = 0; i < pObjects.Length; i++)
        {
            pObjects[i] = new ParticleObject(CubeMaterial);
        }
    }

    private void Update()
    {
        float radius = 440f;
        float scale = .025f;
        float timeScale = 1f;
        float yRotationScale = 1f;

        if (Busker.Instance != null)
        {
            radius = (Busker.Instance.valueOne / 880f) * .1f;
            scale = Mathf.Clamp((Busker.Instance.valueTwo / 880f) / 5f, .0001f, 1f) * .1f;
            timeScale = (((Busker.Instance.valueTempo - 30) / 1970f) * 4f) - 2f; 
            yRotationScale = (Busker.Instance.valuePitchRange - 50) / 1550;
        }

        timeOfSimulation += Time.deltaTime * timeScale;

        if (DataObject.Instance.ARObjectToTrack == null)
            return;

        for (int i = 0; i < pObjects.Length; i++)
        {
            pObjects[i].UpdateObject(DataObject.Instance.ARObjectToTrack.transform.position, timeOfSimulation, radius, scale, yRotationScale);
        }
    }
}
