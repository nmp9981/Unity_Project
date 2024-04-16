bool MouseInPopUpMenu(Vector3 mousePos)
{
    float minPopUpX = clickPopUp.GetComponent<RectTransform>().position.x - clickPopUp.GetComponent<RectTransform>().sizeDelta.x/2;
    float minPopUpY = clickPopUp.GetComponent<RectTransform>().position.y - clickPopUp.GetComponent<RectTransform>().sizeDelta.y / 2;
    float maxPopUpX = clickPopUp.GetComponent<RectTransform>().position.x + clickPopUp.GetComponent<RectTransform>().sizeDelta.x / 2;
    float maxPopUpY = clickPopUp.GetComponent<RectTransform>().position.y + clickPopUp.GetComponent<RectTransform>().sizeDelta.y / 2;

    if (mousePos.x >= minPopUpX && mousePos.x <= maxPopUpX && mousePos.y >= minPopUpY && mousePos.y <= maxPopUpY) return true;
    return false;
}
