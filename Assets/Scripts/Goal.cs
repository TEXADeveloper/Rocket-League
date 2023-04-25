using UnityEngine;
using System;
using System.Collections;

public class Goal : MonoBehaviour
{
    public static event Action Score;
    [SerializeField] private GameObject explosion;
    private GameObject lastExplosion;

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag.Equals("Ball"))
        {
            lastExplosion = GameObject.Instantiate(explosion, col.transform.position, Quaternion.identity);
            StartCoroutine(WaitForExplosion());
        }
            
    }

    private IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(5f);
        Score?.Invoke();
        Destroy(lastExplosion);
    }
}
