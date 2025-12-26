using TMPro;
using UnityEngine;

public class TutorialTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public void Show(string message)
    {
        text.text = message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
