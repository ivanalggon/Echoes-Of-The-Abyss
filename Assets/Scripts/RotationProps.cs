using UnityEngine;

public class RotationProps : MonoBehaviour
{
    public Vector3 velocidadRotacion = new Vector3(0f, 50f, 0f); // grados por segundo

    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime);
    }
}