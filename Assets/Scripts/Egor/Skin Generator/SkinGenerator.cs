using System.Collections.Generic;
using UnityEngine;

public class SkinGenerator : MonoBehaviour
{
    // Класс для настройки одной части одежды в инспекторе
    [System.Serializable]
    public class ClothingPartConfig
    {
        public string partName = "Название части (например, Шляпа)";
        public Transform boneToAttach; // Кость, к которой крепим
        public PrimitiveType shapeType = PrimitiveType.Cube; // Форма
        public Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f); // Размер
        public Vector3 positionOffset = Vector3.zero; // Смещение от кости
    }

    [Header("Настройки одежды")]
    [Tooltip("Добавь сюда элементы одежды и настрой их размеры")]
    public List<ClothingPartConfig> clothingConfigs = new List<ClothingPartConfig>();

    [Header("Материал")]
    public Shader clothingShader; // Шейдер для одежды

    private List<GameObject> currentClothes = new List<GameObject>();

    void Start()
    {
        if (clothingShader == null) clothingShader = Shader.Find("Mobile/Diffuse");
        GenerateSkin();
    }

    // Кнопка в контекстном меню компонента для теста прямо в редакторе
    [ContextMenu("Сгенерировать скин сейчас")]
    public void GenerateSkin()
    {
        ClearClothes();

        // Генерируем пару случайных цветов
        Color color1 = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.3f, 1f);
        Color color2 = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.3f, 1f);

        for (int i = 0; i < clothingConfigs.Count; i++)
        {
            // Чередуем цвета для разнообразия
            Color partColor = (i % 2 == 0) ? color1 : color2;
            CreatePart(clothingConfigs[i], partColor);
        }

        Debug.Log("Скин сгенерирован по настройкам!");
    }

    private void CreatePart(ClothingPartConfig config, Color color)
    {
        if (config.boneToAttach == null) return;

        // 1. Создаем примитив
        GameObject clothPart = GameObject.CreatePrimitive(config.shapeType);
        clothPart.name = "Generated_" + config.partName;

        // 2. Удаляем коллайдер
        DestroyImmediate(clothPart.GetComponent<Collider>());

        // 3. Привязываем и настраиваем
        clothPart.transform.SetParent(config.boneToAttach);
        clothPart.transform.localPosition = config.positionOffset; // Применяем твое смещение
        clothPart.transform.localRotation = Quaternion.identity;
        clothPart.transform.localScale = config.scale; // Применяем твой масштаб

        // 4. Красим
        Renderer rend = clothPart.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = new Material(clothingShader);
            rend.material.color = color;
        }

        currentClothes.Add(clothPart);
    }

    public void ClearClothes()
    {
        foreach (var cloth in currentClothes)
        {
            if (cloth != null) DestroyImmediate(cloth);
        }
        currentClothes.Clear();
    }
}