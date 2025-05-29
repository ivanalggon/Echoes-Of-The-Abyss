using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public GameObject m1911;
    public GameObject ak74;

    public GameObject gun1UI;
    public GameObject gun2UI;

    private bool haveAnotherWeapon = false;
    private int currentWeaponIndex = 1;

    void Start()
    {
        // Asegurar que el jugador empieza solo con la M1911 equipada
        m1911.SetActive(true);
        ak74.SetActive(false);

        gun1UI.SetActive(true);
        gun2UI.SetActive(false);
    }

    void Update()
    {
        // Cambiar a la M1911
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(1);
        }

        // Cambiar al AK74 (solo si ya se ha recogido)
        if (Input.GetKeyDown(KeyCode.Alpha2) && haveAnotherWeapon)
        {
            EquipWeapon(2);
        }
    }

    void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex == 1) // Activar M1911
        {
            Debug.Log("Equipando M1911");
            m1911.SetActive(true);
            ak74.SetActive(false);
            currentWeaponIndex = 1;
        }
        else if (weaponIndex == 2 && haveAnotherWeapon) // Activar AK74 (solo si está recogida)
        {
            Debug.Log("Equipando AK74");
            m1911.SetActive(false);
            ak74.SetActive(true);
            currentWeaponIndex = 2;
        }

        // Actualizar UI
        gun1UI.SetActive(currentWeaponIndex == 1);
        gun2UI.SetActive(currentWeaponIndex == 2);
    }

    // Método para recoger el AK74 del suelo
    public void PickupWeapon()
    {
        if (haveAnotherWeapon) return; // No permitir recoger más de un arma secundaria

        haveAnotherWeapon = true;
        gun2UI.SetActive(true); // Mostrar icono de AK74 en el inventario

        // Asegurar que el AK74 está desactivado hasta que lo equipe
        ak74.SetActive(false);
    }
}
