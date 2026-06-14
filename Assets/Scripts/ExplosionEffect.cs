using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public GameObject explosionPrefab;

    public void TriggerExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        // Stop Action = Destroy se brine o uklanjanju, ali za svaki slucaj:
        Destroy(explosion, 3f);
    }

}
