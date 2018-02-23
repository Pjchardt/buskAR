using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer : MonoBehaviour
{
    InteractableObject hoverObject = null;
    InteractableObject prevObject = null;

    public SingleUnityLayer ARObjectLayer;

    void Update()
    {
        RaycastHit hit;

        if (Physics.SphereCast(Camera.main.transform.position, .05f, Camera.main.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("AR")))
        {
            InteractableObject temp = hit.collider.gameObject.GetComponent<InteractableObject>();

            if (temp != null)
                ProcessInteraction(temp);
        }

    }

    void ProcessInteraction(InteractableObject i)
    {
        if (hoverObject != null)
        {
            if (hoverObject == i)
                hoverObject.OnHoverStay();
            else
            {
                hoverObject.OnHoverExit();
                i.OnHoverEnter();
                hoverObject = i;
            }
        }
        else
        {
            i.OnHoverEnter();
            hoverObject = i;
        }
    }
}
