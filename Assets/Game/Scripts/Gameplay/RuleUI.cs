using TMPro;
using UnityEngine;

public class RuleUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI ruleText;
    [SerializeField]GameObject wrongHighlighter;
    public TextMeshProUGUI RuleText => ruleText;
    [SerializeField] private RectTransform background;
    [Header("Padding")]
    [SerializeField] private float topPadding = 0.1f;
    [SerializeField] private float bottomPadding = 0.1f;
    public void Init(string rule)
    {
        ruleText.text = rule;
        wrongHighlighter.SetActive(false);
        Refresh();
    }

    public void Highlight(bool highlight)
    {
        wrongHighlighter.SetActive(highlight);
    }

    public void Refresh()
    {

        //ruleText.ForceMeshUpdate();

        //float textHeight = ruleText.preferredHeight;
        //Debug.Log("TEXTHEIGHT " +  textHeight);
        //Vector2 bgSize = background.sizeDelta;
        //bgSize.y = textHeight + topPadding + bottomPadding;
        //background.sizeDelta = bgSize;



        ruleText.ForceMeshUpdate();

        int lineCount = ruleText.textInfo.lineCount;

        float lineHeight = ruleText.fontSize * ruleText.lineSpacing * 0.01f;
        float fallbackLineHeight = ruleText.fontSize * 1.2f;

        // TMP lineSpacing can be zero  fallback
        float actualLineHeight = lineHeight > 0 ? lineHeight : fallbackLineHeight;

        float textHeight = lineCount * actualLineHeight;

        Vector2 bgSize = background.sizeDelta;
        bgSize.y = textHeight + topPadding + bottomPadding;
        background.sizeDelta = bgSize;



        //ruleText.ForceMeshUpdate();

        //var textInfo = ruleText.textInfo;

        //float height = 0f;

        //for (int i = 0; i < textInfo.lineCount; i++)
        //{
        //    var line = textInfo.lineInfo[i];
        //    height += line.lineHeight;
        //}

        //Vector2 bgSize = background.sizeDelta;
        //bgSize.y = height + topPadding + bottomPadding;
        //background.sizeDelta = bgSize;

        Debug.Log("TEXTHEIGHT " + textHeight);
    }
}
