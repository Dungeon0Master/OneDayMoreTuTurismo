using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceDetailPanel : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI energyText;  
    [SerializeField] private TextMeshProUGUI satisfactionText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Categorías (solo iconos)")]
    [SerializeField] private Image[] categorySlots;
    [SerializeField] private CategoryIconLibrary categoryIcons;

    public void Show(PlaceInfo info)
    {
        gameObject.SetActive(true);

        nameText.text = info.DisplayName;
        descriptionText.text = info.Description;

        // Los iconos del emoji, el reloj y la moneda van en la UI, por eso aquí solo van números
        satisfactionText.text = $"+{GameSession.Archetype.CalculateSatisfaction(info)}";
        timeText.text = info.TimeMinutes > 0 ? $"-{UIFormat.Minutes(info.TimeMinutes)}" : "0";
        costText.text = info.Cost > 0 ? $"-{info.Cost}" : "0";
        if (energyText != null)
           energyText.text = UIFormat.Signed(info.Energy);

        ShowCategories(info.Categories);

        // Evita un parpadeo del layout al cambiar los textos
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowCategories(PlaceCategory[] categories)
    {
        for (int i = 0; i < categorySlots.Length; i++)
        {
            bool hasCategory = categories != null && i < categories.Length;
            categorySlots[i].gameObject.SetActive(hasCategory);

            if (hasCategory)
                categorySlots[i].sprite = categoryIcons != null ? categoryIcons.Get(categories[i]) : null;
        }
    }
}