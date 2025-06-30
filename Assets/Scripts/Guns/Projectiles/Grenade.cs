using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    public SphereCollider explosionCollider; // solo el componente, no un hijo
    public float explosionDuration = 0.2f;

    void Start()
    {
        explosionCollider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        explosionCollider.enabled = true;
        yield return new WaitForSeconds(explosionDuration);
        explosionCollider.enabled = false;
        Destroy(gameObject);
    }
}
