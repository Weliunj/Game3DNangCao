using UnityEngine;

public class Cube : MonoBehaviour
{
    private Rigidbody rb;
    public float lifetime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(5f, 10f);
        float z = Random.Range(-5f, 5f);
        rb.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, lifetime);
    }
}
