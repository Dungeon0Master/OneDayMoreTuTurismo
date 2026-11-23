using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlaceMapIcon : MonoBehaviour
{
    [SerializeField] private Image categoryImage;   // el dibujo dentro del pin

    public void Setup(Place place, Sprite categorySprite, Action<Place> onClick)
    {
        categoryImage.sprite = categorySprite;
        categoryImage.enabled = categorySprite != null;

        // Atenuado si ya no se puede usar (ya visitado)
        Color tint = place.CanUse ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        GetComponent<Image>().color = tint;
        categoryImage.color = tint;

        GetComponent<Button>().onClick.AddListener(() => onClick(place));
    }
}