using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    private Dictionary<string, string> textDictionary = new Dictionary<string, string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTexts();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadTexts()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "texts.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            var textData = JsonUtility.FromJson<TextData>(jsonData);

            // Копируем все поля из TextData в словарь
            var fields = typeof(TextData).GetFields();
            foreach (var field in fields)
            {
                string value = field.GetValue(textData) as string;
                if (value != null)
                {
                    textDictionary[field.Name] = value;
                }
            }
        }
        else
        {
            Debug.LogError("Texts file not found: " + filePath);
        }
    }

    public string GetText(string key)
    {
        if (textDictionary.ContainsKey(key))
        {
            return textDictionary[key];
        }

        Debug.LogWarning($"Text key not found: {key}");
        return $"MISSING: {key}";
    }
}

// Вспомогательный класс для парсинга JSON
[System.Serializable]
public class TextData
{
    public string welcome;
    public string guard_greeting;
    public string tutorial_move;
    public string item_pickup;
    // Добавляй новые ключи как public поля здесь
}