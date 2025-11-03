using System.Collections.Generic;
using UnityEngine;

public enum DeckLetters
{
    SH,
    I,
    V
}

public class DeckController : MonoBehaviour
{
    [SerializeField] Transform eye;
    [SerializeField] float bigDistance;
    [SerializeField] bool near;
    public TextPanel textPanel;

    Dictionary<DeckLetters, string> trueDic = new Dictionary<DeckLetters, string>()
    {
        { DeckLetters.SH, "Щ"},
        { DeckLetters.I, "I"},
        { DeckLetters.V, "V"}
    };

    Dictionary<DeckLetters, string> falseDic = new Dictionary<DeckLetters, string>()
    {
        { DeckLetters.SH, "ЦЦ"},
        { DeckLetters.I, "II"},
        { DeckLetters.V, "W"}
    };

    private void Start()
    {
        if (textPanel == null) textPanel = G.textPanel;
    }

    public void B1()
    {
        Request(DeckLetters.SH);
    }
    public void B2()
    {
        Request(DeckLetters.I);
    }

    public void B3()
    {
        Request(DeckLetters.V);
    }

    public void Request(DeckLetters deckLetter)
    {

        Vector2 eyePos = eye.position;
        Vector2 pos = transform.position;

        near = (eyePos - pos).magnitude < bigDistance;
        string answer;


        if ((near && G.curentDiagnosis.diseases[(int)Diseases.LongRange]) || (!near && G.curentDiagnosis.diseases[(int)Diseases.ShortRange]))
        {
            answer = "Не вижу";
        }
        else
        {
            answer = G.curentDiagnosis.diseases[(int)Diseases.Focus] ? falseDic[deckLetter] : trueDic[deckLetter];
        }
        Debug.Log(answer);

        textPanel.ShowText(answer);

    }
}
