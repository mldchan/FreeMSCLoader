//Heavily modified Unity UI extensions (old ass version) (BSD3 license)

using UnityEngine.UI;

namespace MSCLoader;

[RequireComponent(typeof(RectTransform), typeof(Button))]
internal class DropDownListButton
{
    public Button btn;
    public Image btnImg;
    public GameObject gameobject;
    public Image img;
    public RectTransform rectTransform;
    public Text txt;

    public DropDownListButton(GameObject btnObj)
    {
        gameobject = btnObj;
        rectTransform = btnObj.GetComponent<RectTransform>();
        btnImg = btnObj.GetComponent<Image>();
        btn = btnObj.GetComponent<Button>();
        txt = rectTransform.FindChild("Text").GetComponent<Text>();
        img = rectTransform.FindChild("Image").GetComponent<Image>();
    }
}