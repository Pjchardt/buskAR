using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for object that can be clicked. ARCore_ViewerController does a sphere cast in update.
/// </summary>
public class InteractableObject : MonoBehaviour {

	protected Material defaultMaterial;
	public Material onHoverMaterial;
	protected Renderer r;

	protected virtual void Awake ()
    {
		r = GetComponent<Renderer> ();
		defaultMaterial = r.material;
	}

	public virtual void OnClick()
	{
		
	}

	public virtual void OnHoverEnter()
	{
		r.material = onHoverMaterial;
	}

	public virtual void OnHoverStay()
	{
		
	}

	public virtual void OnHoverExit()
	{ 
		r.material = defaultMaterial;
	}
}
