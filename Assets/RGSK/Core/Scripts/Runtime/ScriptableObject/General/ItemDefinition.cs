using UnityEngine;

namespace RGSK
{
    public class ItemDefinition : UniqueIDScriptableObject
    {
        public string objectName;
        [TextArea(5, 5)]
        public string description;
        public Sprite icon;
        public Sprite previewPhoto;
        public bool unlockedByDefault;

        [ContextMenu("Unlock Item")]
        public void Unlock()
        {
            if (!SaveData.Instance.unlockedItems.Contains(ID))
            {
                SaveData.Instance.unlockedItems.Add(ID);
            }
        }

        [ContextMenu("Lock Item")]
        public void Lock()
        {
            if (SaveData.Instance.unlockedItems.Contains(ID))
            {
                SaveData.Instance.unlockedItems.Remove(ID);
            }
        }

        public bool IsUnlocked()
        {
            if (unlockedByDefault)
            {
                Unlock();
            }

            return SaveData.Instance.unlockedItems.Contains(ID);
        }
    }
}