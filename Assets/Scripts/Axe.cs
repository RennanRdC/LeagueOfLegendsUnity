using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Axe : MonoBehaviour
{
    [Header("Parameters")]
    public float RotationSpeed;
    public float speed;
    public bool empowered;
    public bool going = true;

    [Header("Players")]
    public Actor target;
    public Draven owner;

    [Header("VFX")]
    public Transform axe;
    public GameObject axeSymbol;
    public GameObject trail;
    public Transform mySymbol;



    // Update is called once per frame
    void Update()
    {
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
                    StartCoroutine(Back(1.6f));
                    trail.SetActive(true);
				} else
				{
                    owner.PlayClip(1);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public IEnumerator Back(float time)
	{
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
