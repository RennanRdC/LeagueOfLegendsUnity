using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draven : Actor
{
    [Header("Axes")]
    public GameObject axePrefab;
    public int axes;

    [Header("VFX and SFX")]
    public ParticleSystem catchAxe;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        anim.SetFloat("axes", axes);
    }

    public void PickAxe()
    {
        if (axes < 2)
        {
            PlayClip(4);
            catchAxe.Play();
            axes++;
        }
    }

    public void InstantiateAxe()
    {
        if (target)
        {
            Axe axe = GameObject.Instantiate(axePrefab).GetComponent<Axe>();
            axe.transform.position = getCenter().position;

            axe.target = target;
            axe.owner = this;

            if (axes > 0)
            {
                axe.empowered = true;
                axes--;
            }

        }
    }

    public Vector3 GetFuturePosition(float time)
    {
        Vector3 futurePosition = nav.destination;

        if (Vector3.Distance(futurePosition, transform.position) > range)
        {
            Vector3 dir = (nav.destination - transform.position).normalized;
            futurePosition = transform.position + (dir * (nav.speed * (time-0.45f)));
        }

        if (Vector3.Distance(futurePosition, transform.position) <= 0.15f)
        {
            Vector3 random = Random.insideUnitSphere * 2;
            futurePosition = new Vector3(random.x, 0, random.z) + transform.position;
        }

        return futurePosition;
    }

    public override void QSkill()
    {
        PickAxe();
    }
}
