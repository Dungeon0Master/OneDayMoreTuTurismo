using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ArchetypeOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.82f, 0.78f, 0.94f);

    private Image background;

    public string Id { get; private set; }

    public void Setup(string id, ArchetypeInfo info, Action<string> onClick)
    {
        Id = id;
        background = GetComponent<Image>();
        label.text = info.DisplayName;

        GetComponent<Button>().onClick.AddListener(() => onClick(id));
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }
}