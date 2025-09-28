using UnityEngine;
using Lean.Pool;
using System.Collections;

public class DestroyObjectAfterSeconds : MonoBehaviour
{
    [Header("Delay to Destroy Variable")]
    private float durToDestroy;
    public float minDurationToDestroy;
    public float maxDurationToDestroy;

    [Header("General Variable")]
    private float beforeDuration;
    public bool isCalledByAnimator;
    public bool isDespawnByNonActivateObject;
    public bool isDestroyingObject;

    private void OnEnable()
    {
        durToDestroy = Random.Range(minDurationToDestroy, maxDurationToDestroy);

        if(isCalledByAnimator == false)
        {
            if(beforeDuration == 0)
            {
                beforeDuration = durToDestroy;
            }

            if(durToDestroy < beforeDuration)
            {
                durToDestroy = beforeDuration;
            }
        }
    }

    void Update()
    {
        if (isCalledByAnimator == false)
        {
            if (durToDestroy > 0)
            {
                durToDestroy -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(DespawnObject());
            }
        }
    }

    public void SetDurationToDestroy(float duration)
    {
        durToDestroy = duration;
    }

    IEnumerator DespawnObject()
    {
        yield return new WaitForSeconds(0f);
        if (isDespawnByNonActivateObject)
        {
            gameObject.SetActive(false);
        }
        else if (isDestroyingObject)
        {
            Destroy(gameObject);
        }
        else
        {            
            LeanPool.Despawn(gameObject);
        }
    }
}
