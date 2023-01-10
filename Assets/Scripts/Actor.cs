using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected NavMeshAgent nav;
    [SerializeField] Transform center;
    [SerializeField] protected AudioSource audioSource;

    [Header("Combat")]
    [SerializeField] protected Actor target;
    [SerializeField] protected float range;

    [Header("VFX and SFX")]
    public ParticleSystem blood;
    public List<AudioClip> clips;

    [Header("Coroutines")]
    Coroutine resetAutoCorou;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        anim.SetFloat("velocity", nav.velocity.magnitude / nav.speed);
        

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (nav.enabled == true)
                nav.isStopped = true;
        }
        else
        {
            if (nav.enabled == true)
                nav.isStopped = false;
        }


        if (target != null)
		{
			if (TargetInRange())
			{
                nav.destination = transform.position;

                SmoothLook(target.transform);

                anim.SetTrigger("Attack");
            } 
            else
			{
                nav.destination = target.transform.position;
            }
		}
    }

    public void SetDestiny(Vector3 destiny)
	{
        SetTarget(null);

        if (resetAutoCorou != null)
		{
            StopCoroutine(resetAutoCorou);
            resetAutoCorou = null;
            anim.SetBool("ResetAuto", false);
        }
		else
		{
            resetAutoCorou = StartCoroutine(ResetAuto());
        }

        nav.destination = destiny;
    }

    public IEnumerator ResetAuto()
    {
        anim.ResetTrigger("Attack");

        anim.SetBool("ResetAuto", true);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("ResetAuto", false);

        resetAutoCorou = null;
    }
    public void SetTarget(Actor targ)
    {
        anim.ResetTrigger("Attack");
        target = targ;
    }

    public bool TargetInRange()
    {
        if (target != null)
            return (Vector3.Distance(this.transform.position, target.transform.position) <= range);
        
        return false;
    }

    private void SmoothLook(Transform targTransf)
    {
        Vector3 heading = targTransf.position - transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        Quaternion look = Quaternion.LookRotation(direction);
        look.eulerAngles = new Vector3(0, look.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.deltaTime * 10);
    }

    public void PlayClip(int index)
	{
        audioSource.PlayOneShot(clips[index]);
	}


    public Transform getCenter()
	{
        return center;
	}

    public virtual void QSkill(){}
}
