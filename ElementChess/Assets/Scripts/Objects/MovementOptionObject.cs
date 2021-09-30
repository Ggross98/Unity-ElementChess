using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementOptionObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private Image icon, background;

    [SerializeField]
    private Text typeText, countText;

    public enum OptionState { NORMAL, HIGHLIGHTED, SELECTED, DISABLED }

    private OptionState state = OptionState.NORMAL;


    public bool disabled = false, selected = false;

    public int count;

    public ChessData.Type type;

    private Color normalColor, highlightedColor, selectedColor, disabledColor;

    public void SetState(OptionState s)
    {
        state = s;
        switch (s)
        {
            case OptionState.NORMAL:
                background.color = normalColor;
                break;
            case OptionState.HIGHLIGHTED:
                background.color = highlightedColor;
                break;
            case OptionState.DISABLED:
                background.color = disabledColor;
                break;
            case OptionState.SELECTED:
                background.color = selectedColor;
                break;
        }
    }

    public void Select(bool _s)
    {
        selected = _s;
        SetState(_s ? OptionState.SELECTED : OptionState.NORMAL);
    }

    public void Disable(bool _b)
    {
        disabled = _b;
        SetState(_b ? OptionState.DISABLED : OptionState.NORMAL);
    }

    public void SetColor(Color nc, Color hc, Color sc, Color dc)
    {
        normalColor = nc;
        highlightedColor = hc;
        selectedColor = sc;
        disabledColor = dc;

        background.color = normalColor;
    }

    public void SetCount(int i)
    {
        count = i;
        countText.text = i + "";

        
    }

    public void CheckDisable()
    {
        Disable(count <= 0);
    }

    public void SetIcon(Sprite s)
    {
        icon.sprite = s;

        icon.gameObject.SetActive(s!=null);
    }

    public void SetName(string name)
    {
        typeText.text = name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!selected && !disabled)
        {
            MovementController.Instance.SetSelectedOption(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!selected && !disabled)
        {
            SetState(OptionState.HIGHLIGHTED);
            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected && !disabled)
        {
            SetState(OptionState.NORMAL);

        }
    }
}
