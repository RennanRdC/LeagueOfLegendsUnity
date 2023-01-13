using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draven : Actor
{
    [Header("Axes")]
    [SerializeField] GameObject axePrefab;
    private int axes;

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

    //Called when pick axe or activate Q skill
    public void PickAxe()
    {
        if (axes < 2)
        {
            PlayClip(4);
            catchAxe.Play();
            axes++;
        }
    }

    //Auto attack projectille
    public void InstantiateAxe()
    {
        if (target)
        {
            Axe axe = GameObject.Instantiate(axePrefab).GetComponent<Axe>();
            axe.transform.position = getCenter().position;

            bool empowered = false;

            if (axes > 0)
            {
                empowered = true;
                axes--;
            }

            axe.Initialize(target,this,empowered);

        }
    }

    //Calculates the position where the character will be in X seconds
    public Vector3 GetFuturePosition(float time)
    {
        Vector3 futurePosition = nav.destination;

        if (Vector3.Distance(futurePosition, transform.position) > range)
        {
            Vector3 dir = (nav.destination - transform.position).normalized;
            futurePosition = transform.position + (dir * (nav.speed * (time) *0.7f));
        }

        if (Vector3.Distance(futurePosition, transform.position) <= 0.15f)
        {
            Vector3 random = Random.insideUnitSphere * 2;
            futurePosition = new Vector3(random.x, 0, random.z) + transform.position;
        }

        return futurePosition;
    }


    //Skills
    public override void QSkill()
    {
        PickAxe();
    }
}
