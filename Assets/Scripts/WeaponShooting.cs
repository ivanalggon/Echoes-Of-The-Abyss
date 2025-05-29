using UnityEngine;
using System.Collections;
using TMPro;

public class WeaponShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public int maxAmmo = 25;
    private int currentAmmo;
    public Animator animator;  // Arrastra aquí el Animator en el inspector


    public int reloadTime = 2;
    private bool isReloading = false;

    public TextMeshProUGUI ammotext;
    public TextMeshProUGUI reloadtext;

    public LayerMask clickLayerMask; // Asignar solo el suelo o plano donde se puede disparar

    void Start()
    {
        ammotext.gameObject.SetActive(true);
        reloadtext.gameObject.SetActive(false);
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading)
        {
            ShootTowardsMouse();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        UpdateAmmoText();
    }

    void ShootTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickLayerMask))
        {
            // Obtener punto de impacto
            Vector3 hitPoint = hit.point;

            // Calcular dirección horizontal (ignorar diferencia de altura si estás en top-down)
            Vector3 direction = (hitPoint - firePoint.position);
            direction.y = 0f; // muy importante para top-down
            direction = direction.normalized;

            // Instanciar y disparar la bala
            currentAmmo--;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(1);
            }

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }

            Destroy(bullet, 3f);
        }
    }


    private IEnumerator Reload()
    {
        isReloading = true;
        ammotext.gameObject.SetActive(false);
        reloadtext.gameObject.SetActive(true);

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;

        isReloading = false;
        ammotext.gameObject.SetActive(true);
        reloadtext.gameObject.SetActive(false);
    }

    private void UpdateAmmoText()
    {
        if (ammotext != null)
        {
            ammotext.text = currentAmmo + "/" + maxAmmo;
        }
    }
}
