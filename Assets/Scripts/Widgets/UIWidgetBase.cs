using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIWidgetBase : MonoBehaviour
{
    protected TMP_Text FindTextByName(TMP_Text[] texts, params string[] tokens)
    {
        foreach (TMP_Text text in texts)
        {
            if (text == null) continue;
            string lower = text.gameObject.name.ToLowerInvariant();
            foreach (string token in tokens)
            {
                if (lower.Contains(token)) return text;
            }
        }
        return null;
    }

    protected Image FindImageByName(Image[] images, string token)
    {
        foreach (Image image in images)
        {
            if (image == null) continue;
            if (image.gameObject.name.ToLowerInvariant().Contains(token)) return image;
        }
        return null;
    }

    protected T FindComponentByName<T>(T[] components, string token) where T : Component
    {
        foreach (T component in components)
        {
            if (component == null) continue;
            if (component.gameObject.name.ToLowerInvariant().Contains(token)) return component;
        }
        return null;
    }
}
