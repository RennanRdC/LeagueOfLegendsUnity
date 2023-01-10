using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
	public Actor actor;
	public LayerMask selectLayer;
	public GameObject waypoint;


    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Q))
		{
			actor.QSkill();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			actor.SetTarget(null);
		}


		if (Input.GetMouseButtonDown(1))
		{

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, selectLayer);

			if (hit)
			{
				GameObject hited = hitInfo.transform.gameObject;

				switch (hited.layer)
				{
						case 6:
							actor.SetDestiny(hitInfo.point);						
							InstantiateWay(hitInfo.point);
						break;

						case 7:
							Actor selectchar = hited.GetComponent<Actor>();
							if(actor != selectchar)
							{
								actor.SetTarget(selectchar);
							}
							
						break;
				}
			}
		}
	}


	public void InstantiateWay(Vector3 pos)
	{
		Transform axe = GameObject.Instantiate(waypoint).transform;
		axe.transform.position = pos;
		axe.GetComponentInChildren<SpriteRenderer>().DOColor(Color.clear, 0.5f);

		Destroy(axe.gameObject, 0.5f);
	}
}
