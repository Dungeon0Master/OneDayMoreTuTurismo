using System;
using System.Collections.Generic;

[Serializable]
public class SaveSection
{
    public string key;
    public string json;
}

[Serializable]
public class SaveFile
{
    public int version;
    public string savedAtUtc;
    public string runId;
    public string archetypeId;
    public string checksum;
    public List<SaveSection> sections = new List<SaveSection>();

    public SaveSection Find(string key)
    {
        foreach (SaveSection section in sections)
        {
            if (section.key == key) return section;
        }
        return null;
    }
}