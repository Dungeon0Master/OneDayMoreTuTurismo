using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Tu-Turismo/Iconos de categoría", fileName = "CategoryIconLibrary")]
public class CategoryIconLibrary : ScriptableObject
{
    [Serializable]
    private struct Entry
    {
        public PlaceCategory category;
        public Sprite sprite;
    }

    [SerializeField] private Entry[] entries;

    public Sprite Get(PlaceCategory category)
    {
        if (entries == null) return null;

        foreach (Entry entry in entries)
        {
            if (entry.category == category) return entry.sprite;
        }
        return null;
    }
}