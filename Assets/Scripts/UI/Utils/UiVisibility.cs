using UnityEngine;

namespace UI.Utils
{
    public static class UiVisibility
    {
        public static void ShowWithParents(Transform target)
        {
            Transform current = target.parent;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                    current.gameObject.SetActive(true);

                CanvasGroup parentCG = current.GetComponent<CanvasGroup>();
                if (parentCG != null)
                {
                    parentCG.alpha = 1f;
                    parentCG.interactable = true;
                    parentCG.blocksRaycasts = true;
                }

                current = current.parent;
            }
        }
    }
}
