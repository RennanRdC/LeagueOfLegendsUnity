using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Axe : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float RotationSpeed;
    [SerializeField] float speed;
    [SerializeField] bool empowered;
    bool going = true;

    [Header("Players")]
    [SerializeField] Actor target;
    [SerializeField] Draven owner;

    [Header("VFX")]
    [SerializeField] Transform axe;
    [SerializeField] GameObject axeSymbol;
    [SerializeField] GameObject trail;
    [SerializeField] Transform mySymbol;

    //Initialize Axe parameters
	public void Initialize(Actor p_target, Draven p_owner, bool p_empowered)
	{
        target = p_target;
        owner = p_owner;
        empowered = p_empowered;
	}

	// Update is called once per frame
	void Update()
    {
        //Axe rotation
        axe.Rotate(Vector3.forward * (RotationSpeed * Time.deltaTime));

        if (going)
		{
            transform.position = Vector3.MoveTowards(transform.position, target.getCenter().position, Time.deltaTime * speed);
            transform.LookAt(target.transform.position);

            if (Vector3.Distance(transform.position, target.getCenter().position) < 0.02f)
            {
                target.blood.Play();

                if(empowered)
				{
                    owner.PlayClip(2);
                    owner.PlayClip(3);
                    //If empowered back to Draven
                    StartCoroutine(Back(1.6f));
				} else
				{
                    owner.PlayClip(1);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    //Uses Draven GetFuturePosition to return the axe to the predicted position
    public IEnumerator Back(float time)
	{
        trail.SetActive(true);

        Vector3 futurePosition = owner.GetFuturePosition(time);

        mySymbol = GameObject.Instantiate(axeSymbol).transform;
        mySymbol.transform.position = new Vector3(futurePosition.x, 0, futurePosition.z);

        transform.LookAt(futurePosition);

        going = false;

        transform.DOJump(futurePosition, 3, 1, time);

        yield return new WaitForSeconds(time-0.35f);

        if (mySymbol != null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(mySymbol.GetComponentInChildren<SpriteRenderer>().DOColor(Color.red, 0.1f));
            sequence.Append(mySymbol.GetComponentInChildren<SpriteRenderer>().DOColor(Color.clear, 0.3f));
            Destroy(mySymbol.gameObject,0.4f);
        }

        owner.PlayClip(0);

        Destroy(this.gameObject);
     
    }


    //Used to check if any Draven catch the axe
	private void OnTriggerEnter(Collider other)
	{
		if(!going)
		{
            if (mySymbol != null)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(mySymbol.GetComponentInChildren<SpriteRenderer>().DOColor(Color.clear, 0.2f));

                Destroy(mySymbol.gameObject,0.2f);
            }

            Draven draven = other.GetComponent<Draven>();

			if (draven)
			{
                draven.PickAxe();
            }

            Destroy(this.gameObject);
        }
	}
}
