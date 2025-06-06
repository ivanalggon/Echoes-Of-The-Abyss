using UnityEngine;
using System.Collections;
using TMPro;

public class WeaponShooting : MonoBehaviour
{
    public GameObject bulletPrefabNormal;
    public GameObject bulletPrefabMejorado;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public int maxAmmo = 25;
    private int currentAmmo;
    public Animator animator;

    public int reloadTime = 2;
    private bool isReloading = false;

    public AudioClip shootSound;
    private AudioSource audioSource;

    public TextMeshProUGUI ammotext;
    public TextMeshProUGUI reloadtext;

    public LayerMask clickLayerMask;

    private bool hasDamageUpgrade = false;

    void Start()
    {
        ammotext.gameObject.SetActive(true);
        reloadtext.gameObject.SetActive(false);
        currentAmmo = maxAmmo;
        UpdateAmmoText();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading)
        {
            ShootTowardsMouse();
            if (shootSound != null)
                audioSource.PlayOneShot(shootSound);
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
            Vector3 hitPoint = hit.point;
            Vector3 direction = (hitPoint - firePoint.position);
            direction.y = 0f;
            direction = direction.normalized;

            currentAmmo--;

            GameObject prefabToShoot = hasDamageUpgrade ? bulletPrefabMejorado : bulletPrefabNormal;
            GameObject bullet = Instantiate(prefabToShoot, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
                rb.velocity = direction * bulletSpeed;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
                bulletScript.SetDamage(1); // Si cada prefab ya tiene daño, puedes omitir esta línea

            if (currentAmmo <= 0)
                StartCoroutine(Reload());

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

    public void ActivateDamageUpgrade()
    {
        hasDamageUpgrade = true;
    }
}
