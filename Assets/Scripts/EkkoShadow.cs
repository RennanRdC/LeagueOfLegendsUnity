using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EkkoShadow : MonoBehaviour
{
    Animator anim;
    Vector3 destiny;
    Ekko owner;

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	//Initialize main ekko's shadow
	public void Initialize(Ekko p_owner)
	{
        owner = p_owner;
        destiny = owner.transform.position;
		transform.position = owner.transform.position;
		transform.rotation = owner.transform.rotation;
	}

	//Set next shadow destination
    public void SetDestination(PositionRotation p_destiny, float p_tickInterval)
	{
		anim.SetFloat("velocity", p_destiny.position != destiny? 1 : 0);
		destiny = p_destiny.position;
        transform.DOMove(destiny, p_tickInterval).SetEase(Ease.Linear);
		transform.DORotateQuaternion(p_destiny.rotation, 0.1f);
	}


	//Initialize static shadow vfx
	public void InitializeStaticShadow(PositionRotation p_destiny)
	{
		transform.position = p_destiny.position;
		transform.rotation = p_destiny.rotation;

		anim.SetFloat("velocity", 1);
		anim.Play("Base Layer.WalkRun", 0, Random.Range(0.00f,1.00f));
		anim.speed = 0;

		Material m_Material = GetComponentInChildren<Renderer>().material;
		m_Material.DOColor(Color.clear, "_TintColor", 2f);
		Destroy(this.gameObject, 2f);
	}
}
