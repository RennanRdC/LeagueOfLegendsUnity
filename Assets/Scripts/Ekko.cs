using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionRotation
{
    public Vector3 position;
    public Quaternion rotation;

    public PositionRotation (Vector3 p_position, Quaternion p_rotation)
	{
        position = p_position;
        rotation = p_rotation;
	}
}

public class Ekko : Actor
{
    [Header("Ultimate")]
    [SerializeField][Range(1f,10f)] float rewindTime = 4f;
    [SerializeField] [Range(1, 50)] int rewindTicks = 10;
    List<PositionRotation> pathPoints = new List<PositionRotation>();
    bool ultimateOn;
    Coroutine ultimateCoroutine;

    [Header("VFX and SFX")]
    [SerializeField] GameObject EkkoShadowPrefab;
    [SerializeField] LineRenderer myShadowLineRenderer;
    [SerializeField] ParticleSystem grondReticles;
    [SerializeField] ParticleSystem StartUlt;
    [SerializeField] GameObject trailBack;
    [SerializeField] GameObject ultimateVFX;
    EkkoShadow myShadow;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

		if (ultimateOn)
		{
            UpdateLineRenderer();
		}


        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ult1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Ult2"))
        {
            if (nav.enabled == true)
                nav.isStopped = true;
        }
        else
        {
            if (nav.enabled == true)
                nav.isStopped = false;
        }

    }

    public void Attack()
	{
        target.spark.Play();
    }

    public override void QSkill()
    {
        InitializeUltimate();
    }

    public override void RSkill()
    {
        StartCoroutine(UseUltimate());
    }

    public void UltParticle()
    {
        StartUlt.Play();
    }


    void InitializeUltimate()
	{
        if (ultimateOn) return;

        ultimateOn = true;

		for (int i = 0; i < rewindTicks; i++)
		{
            pathPoints.Add(new PositionRotation(transform.position,transform.rotation));
		}

        grondReticles.Play();
        trailBack.SetActive(true);

        myShadow = GameObject.Instantiate(EkkoShadowPrefab).GetComponent<EkkoShadow>();
        myShadow.Initialize(this);

        ultimateCoroutine = StartCoroutine(UltimateTickCoroutine());
	}

    IEnumerator UltimateTickCoroutine()
    {
        float tickInterval = rewindTime / rewindTicks;

        while (ultimateOn)
        {
            yield return new WaitForSeconds(tickInterval);
            pathPoints.RemoveAt(0);
            pathPoints.Add(new PositionRotation(transform.position, transform.rotation));

            myShadow.SetDestination(pathPoints[0], tickInterval);
        }
    }

    IEnumerator UseUltimate()
	{
        ultimateOn = false;
        StopCoroutine(ultimateCoroutine);
        myShadowLineRenderer.positionCount = 0;

        StaticShadows();

        pathPoints.Clear();

        grondReticles.Stop();
        trailBack.SetActive(false);

        Vector3 shadowPosition = myShadow.transform.position;
        Quaternion shadowRotation = myShadow.transform.rotation;

        Destroy(myShadow.gameObject);

        anim.SetTrigger("Ult");

        PlayClip(1);

        Transform ultVFX = GameObject.Instantiate(ultimateVFX).transform;
        ultVFX.position = shadowPosition + new Vector3(0, 0.5f, 0);
        

        Destroy(ultVFX.gameObject, 2.3f);

        yield return new WaitForSeconds(0.6f);
        PlayClip(2);

        transform.position = shadowPosition;
        transform.rotation = shadowRotation;
        SetDestiny(shadowPosition);
    }

    void UpdateLineRenderer()
	{
        float height = getCenter().position.y - transform.position.y;

        myShadowLineRenderer.positionCount = pathPoints.Count+2;

        myShadowLineRenderer.SetPosition(0, new Vector3(0, height, 0) + myShadow.transform.position);

        for (int i = 0; i < pathPoints.Count; i++)
		{
            myShadowLineRenderer.SetPosition(i+1, new Vector3(0, height, 0) + pathPoints[i].position);
		}

        myShadowLineRenderer.SetPosition(pathPoints.Count+1, new Vector3(0, height, 0) + transform.position);

    }

    public void StaticShadows()
	{
        EkkoShadow currentShadowEkko = GameObject.Instantiate(EkkoShadowPrefab).GetComponent<EkkoShadow>();

        currentShadowEkko.InitializeStaticShadow(new PositionRotation(transform.position,transform.rotation));


        for (int i = 0; i < pathPoints.Count; i++)
		{
            EkkoShadow currentShadow = GameObject.Instantiate(EkkoShadowPrefab).GetComponent<EkkoShadow>();

            currentShadow.InitializeStaticShadow(pathPoints[i]);
        }
    }
}
