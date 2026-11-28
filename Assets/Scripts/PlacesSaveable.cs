using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacesSaveable : MonoBehaviour, ISaveable
{
    [Serializable]
    private class State
    {
        public List<string> usedPlaceIds = new List<string>();
    }

    public string SaveKey => "places";
    public int RestoreOrder => 10;   // después de las carpas, que también son Place

    public string CaptureState()
    {
        State state = new State();

        foreach (Place place in Place.All)
        {
            if (place.Used && !string.IsNullOrEmpty(place.Id))
                state.usedPlaceIds.Add(place.Id);
        }
        return JsonUtility.ToJson(state);
    }

    public void RestoreState(string json)
    {
        State state = JsonUtility.FromJson<State>(json);
        HashSet<string> used = new HashSet<string>(state.usedPlaceIds);

        foreach (Place place in Place.All)
        {
            if (used.Contains(place.Id))
                place.MarkUsed();
        }
    }
}