using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class UniversalPanelManager : MonoBehaviour
{
    [Header("Панелі (key = назва панелі, value = GameObject)")]
    public List<PanelItem> panels = new List<PanelItem>();

    private Dictionary<string, GameObject> panelDict;

    [System.Serializable]
    public class PanelItem
    {
        public string name;
        public GameObject panel;
    }

    private void Awake()
    {
        // Конвертуємо список у словник для швидкого доступу
        panelDict = new Dictionary<string, GameObject>();
        foreach (var item in panels)
        {
            if (!panelDict.ContainsKey(item.name))
                panelDict.Add(item.name, item.panel);
        }
    }

    /// <summary>
    /// Відкрити панель за назвою (інші вимикаються)
    /// </summary>
    public void OpenPanel(string panelName)
    {
        foreach (var panel in panelDict.Values)
            panel.SetActive(false);

        if (panelDict.ContainsKey(panelName))
            panelDict[panelName].SetActive(true);
        else
            Debug.LogWarning("Панель не знайдена: " + panelName);
    }

    /// <summary>
    /// Закрити всі панелі
    /// </summary>
    public void CloseAll()
    {
        foreach (var panel in panelDict.Values)
            panel.SetActive(false);
    }
}
