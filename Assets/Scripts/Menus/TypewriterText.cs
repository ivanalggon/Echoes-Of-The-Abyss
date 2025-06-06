using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterText : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public float typingSpeed = 0.05f;
    public float pauseBetweenParagraphs = 2f;

    [TextArea(3, 5)]
    public string paragraph1 = "Con el coraz�n latiendo con fuerza, Soren logr� arrancar a Vael de las garras de la oscuridad.";

    [TextArea(3, 5)]
    public string paragraph2 = "Juntos, corrieron por los t�neles colapsando, con las sombras pis�ndoles los talones.";

    [TextArea(3, 5)]
    public string paragraph3 = "Aquella figura sombr�a a la que llamaban madre� no era humana.";

    [TextArea(3, 5)]
    public string paragraph4 = "Era una bruja corrompida por la cueva, dispuesta a sacrificar a su propio hijo por poder eterno.";

    [TextArea(3, 5)]
    public string paragraph5 = "Pero no lo logr�. El v�nculo entre hermanos fue m�s fuerte que cualquier hechizo.";

    [TextArea(3, 5)]
    public string paragraph6 = "Y al final, Soren y Vael escaparon sanos y libres.";

    void Start()
    {
        StartCoroutine(ShowAllParagraphs());
    }

    IEnumerator ShowAllParagraphs()
    {
        string[] paragraphs = new string[] { paragraph1, paragraph2, paragraph3, paragraph4, paragraph5, paragraph6 };

        foreach (string paragraph in paragraphs)
        {
            textUI.text = "";  // Borrar texto anterior
            yield return StartCoroutine(TypeText(paragraph));
            yield return new WaitForSeconds(pauseBetweenParagraphs);
        }
    }

    IEnumerator TypeText(string text)
    {
        foreach (char letter in text)
        {
            textUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
