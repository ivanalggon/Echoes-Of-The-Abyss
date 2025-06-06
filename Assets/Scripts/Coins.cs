using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coins : MonoBehaviour
{
    
    public TextMeshProUGUI coinText;
    public int coinValue = 0;

    void Start()
    {
        // Inicializa el texto de las monedas
        coinText.text = "x " + coinValue.ToString();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Incrementa el valor de las monedas
            coinValue += 5;
            coinText.text = "x " + coinValue.ToString();
            Destroy(gameObject);
        }
    }
}
