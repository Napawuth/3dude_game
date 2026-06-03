using UnityEngine;

public class RainDrop : MonoBehaviour
{
    // Mode A: Runs instantly when hitting a solid 3D surface
    private void OnCollisionEnter(Collision collision)
    {
        // This will instantly delete the drop when it touches ANY object
        // (This completely bypasses tag mistakes, layer bugs, and typos)
        Destroy(gameObject);
    }

    // Mode B: Runs instantly if your floor is set to "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}