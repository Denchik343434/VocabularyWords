using UnityEngine;
using System.Collections.Generic;

/*
public class TestDataGenerator : MonoBehaviour
{
    // Позволяет запустить метод через Контекстное меню компонентов в Инспекторе
    [ContextMenu("Generate Test Libraries")]
    public void GenerateTestData()
    {
        // Инициализируем папку перед записью
        StorageManager.InitializeStorage();

        // 1. Библиотека "Cyberpunk Slang"
        LibraryData cyberpunkLib = new LibraryData
        {
            LibraryName = "Cyberpunk Slang",
            Words = new List<WordData>
            {
                new WordData { Word = "Choombatta", Explanation = "Friend or buddy (often shortened to 'choom')." },
                new WordData { Word = "Corpo", Explanation = "A corporate employee or representative, usually highly untrusted." },
                new WordData { Word = "Flatline", Explanation = "To die or to kill someone/something." },
                new WordData { Word = "Preem", Explanation = "Short for premium; best, top tier, or awesome." },
                new WordData { Word = "ICE", Explanation = "Intrusion Countermeasure Electronics; security systems in the Net." }
            }
        };

        // 2. Библиотека "Fantasy Spells" (с краевыми случаями: длинный текст, пустые строки)
        LibraryData fantasyLib = new LibraryData
        {
            LibraryName = "Fantasy Spells",
            Words = new List<WordData>
            {
                new WordData { Word = "Fireball", Explanation = "A bright streak shoots from your pointing finger to a point you choose within range and then blossoms with a low roar into an explosion of flame." },
                new WordData { Word = "LoooongSpellNameThatMightBreakYourUIComponentIfItOverflowsTheBoundsContainer", Explanation = "Тест длинного имени для проверки адаптивности интерфейса и выравнивания текста." },
                new WordData { Word = "Power Word: Kill", Explanation = "You utter a word of power that can compel one creature you can see to die instantly." },
                new WordData { Word = "Empty Description", Explanation = "" } // Крайний случай: пустое описание
            }
        };

        // Сохраняем обе библиотеки в .vcl архивы
        StorageManager.SaveLibrary(cyberpunkLib);
        StorageManager.SaveLibrary(fantasyLib);

        Debug.Log("[TestDataGenerator] Готово! Зайди в папку Libraries или вызови GetLibraryNames() в UI.");
    }

    private void Start()
    {
        GenerateTestData();
    }
}
*/
