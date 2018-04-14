using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNote : InteractableObject
{
    public GameObject ClickEffect;

    public override void OnClick()
    {
        base.OnClick();

        GameObject obj = Instantiate(ClickEffect);
        obj.transform.position = transform.position;
        Destroy(obj, 3f);
    }
}
