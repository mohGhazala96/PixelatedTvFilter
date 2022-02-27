using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelHandler : MonoBehaviour
{
    [HideInInspector]
    public bool enablePhysics = false;
    [HideInInspector]
    public Vector3 targetPosition;
    bool canHit = false;

    // Start is called before the first frame update
    void Start()
    {

    }
    IEnumerator LerpPosition(float duration)
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        transform.localEulerAngles = Vector3.zero;
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        canHit = false;
        Destroy(GetComponent<Rigidbody>());
    }

    IEnumerator Loop()
    {
        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        if (ConfigurationsManager.Instance.force == Force.Fall)
        {
            Physics.gravity = new Vector3(0, -30.0F, 0);
            GetComponent<Rigidbody>().AddForce(new Vector3(0, Random.Range(1, 3),0) * 15, ForceMode.Impulse);
        }
        else if (ConfigurationsManager.Instance.force == Force.FallBack)
        {
            Physics.gravity = new Vector3(0, -40.0F, 0);
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, Random.Range(-1, -7)) * 8, ForceMode.Impulse);
        }
        else if (ConfigurationsManager.Instance.force == Force.FallFront)
        {
            Physics.gravity = new Vector3(0, -40.0F, 0);
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, Random.Range(1, 2.5f)) * 5, ForceMode.Impulse);

        }
        else if (ConfigurationsManager.Instance.force == Force.RandomScatter)
        {
            Physics.gravity = new Vector3(0, -30.0F, 0);
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5, 6), Random.Range(1, 5), Random.Range(-5, 5)) * 10, ForceMode.Impulse);

        }
        yield return new WaitForSeconds(4);
        yield return StartCoroutine(LerpPosition(2));
    }



    // Update is called once per frame
    void Update()
    {
        if (enablePhysics && !canHit)
        {
            enablePhysics = false;
            canHit = true;
            StartCoroutine(Loop());
        }


    }


}
