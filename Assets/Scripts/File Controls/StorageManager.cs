using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

// TODO: добавить сохранение аудио файлов в zip архиве

// скрипт с статичными методами для использования в компонентах, для работы с файлами
public class StorageManager
{
    /*
    //Путь к папке с библиотеками
    private static string _librariesFolderPath => Path.Combine(Application.persistentDataPath, "Libraries");

    // Сохранить библиотеку в zip - .vcl с json
    // думать 
    public static void SaveLibrary(LibraryData library)
    {

        string tempFolder = Path.Combine(Application.temporaryCachePath, library.LibraryName);
        string vclFilePath = Path.Combine(_librariesFolderPath, library.LibraryName + ".vcl");

        // 1. Создаем чистую временную папку для сборки
        if (Directory.Exists(tempFolder))
        {
            Directory.Delete(tempFolder, true);
        }
        Directory.CreateDirectory(tempFolder);

        // 2. Преобразуем объект в JSON и пишем во временную папку
        string jsonText = JsonUtility.ToJson(library, true);
        string jsonPath = Path.Combine(tempFolder, "library.json");
        File.WriteAllText(jsonPath, jsonText);

        // 3. Если старый файл .vcl существует — удаляем
        if (File.Exists(vclFilePath))
        {
            File.Delete(vclFilePath);
        }

        // 4. Пакуем временную папку в .vcl архив
        ZipFile.CreateFromDirectory(tempFolder, vclFilePath);

        // 5. Чистим за собой временную папку
        Directory.Delete(tempFolder, true);

        Debug.Log($"[Storage] Библиотека '{library.LibraryName}.vcl' успешно сохранена!");
    }

    // Загрузить библиотеку по имени
    // оставляем
    public static LibraryData GetLibrary(string libraryName)
    {
        string vclFilePath = Path.Combine(_librariesFolderPath, libraryName + ".vcl");

        string tempFolder = Path.Combine(Application.temporaryCachePath, "Unpacked_" + libraryName);

        // 1. Готовим чистую папку для распаковки
        if (Directory.Exists(tempFolder))
        {
            Directory.Delete(tempFolder, true);
        }

        // 2. Распаковываем .vcl
        ZipFile.ExtractToDirectory(vclFilePath, tempFolder);

        // 3. Читаем library.json из архива
        string jsonPath = Path.Combine(tempFolder, "library.json");
        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"[Storage] В архиве {libraryName}.vcl не найден library.json!");
            Directory.Delete(tempFolder, true);
            return null;
        }
        string jsonText = File.ReadAllText(jsonPath);
        LibraryData loadedLibrary = JsonUtility.FromJson<LibraryData>(jsonText);

        // 4. Чистим за собой временную папку
        Directory.Delete(tempFolder, true);

        return loadedLibrary;
    }

    // Удалить библиотеку по имени
    // оставляем
    public static void DeleteLibrary(string libraryName)
    {
        string vclFilePath = Path.Combine(_librariesFolderPath, libraryName + ".vcl");
        if (File.Exists(vclFilePath))
        {
            File.Delete(vclFilePath);
            Debug.Log($"[Storage] Библиотека '{libraryName}.vcl' успешно удалена!");
        }
        else
        {
            Debug.LogWarning($"[Storage] Библиотека '{libraryName}.vcl' не найдена для удаления!");
        }
    }

    
    public static void AddLibrary(string _libruaryPath)
    {
        File file = new
    }
    

    // Проверяет наличие папки и создает её при необходимости
    // оставляем
    public static void InitializeStorage()
    {
        if (!Directory.Exists(_librariesFolderPath))
        {
            Directory.CreateDirectory(_librariesFolderPath);
            Debug.Log($"[Storage] Папка создана по пути: {_librariesFolderPath}");
        }
    }

    // Получить имена всех библиотек
    public static List<string> GetLibraryNames()
    {
        List<string> libraryNames = new List<string>();
        string[] files = Directory.GetFiles(_librariesFolderPath, "*.vcl");

        foreach (string file in files)
        {
            libraryNames.Add(Path.GetFileNameWithoutExtension(file));
        }

        return libraryNames;
    }
    */


    // Путь к постоянному хранилищу с .vcl архивами
    private static string LibrariesFolderPath => Path.Combine(Application.persistentDataPath, "Libraries");

    // Корневая рабочая папка в кэше для текущей сессии
    private static string CacheFolderPath => Path.Combine(Application.temporaryCachePath, "CacheSession");

    // Внутренние подпапки в кэше для порядка
    private static string CacheJsonsFolderPath => Path.Combine(CacheFolderPath, "JSONs");
    private static string CacheAudioFolderPath => Path.Combine(CacheFolderPath, "Audio");

    /// <summary>
    /// Инициализация хранилища: создаёт папку с библиотеками и чистит старый кэш при запуске.
    /// </summary>
    public static void InitializeStorage()
    {
        if (!Directory.Exists(LibrariesFolderPath))
        {
            Directory.CreateDirectory(LibrariesFolderPath);
            Debug.Log($"[Storage] Папка библиотек создана: {LibrariesFolderPath}");
        }
    }

    /// <summary>
    /// Распаковывает список библиотек по их именам во временный кэш.
    /// </summary>
    public static void UnpackLibrariesToCache(params string[] libraryNames)
    {
        EnsureCacheDirectoriesExist();

        foreach (string libName in libraryNames)
        {
            string vclFilePath = Path.Combine(LibrariesFolderPath, libName + ".vcl");

            if (!File.Exists(vclFilePath))
            {
                Debug.LogError($"[Storage] Файл библиотеки не найден: {vclFilePath}");
                continue;
            }

            try
            {
                // Временная папка для промежуточной распаковки архива
                string tempUnpackPath = Path.Combine(Application.temporaryCachePath, "TempUnpack_" + libName);
                if (Directory.Exists(tempUnpackPath))
                {
                    Directory.Delete(tempUnpackPath, true);
                }

                // 1. Распаковываем zip-архив с поддержкой UTF-8 (для кириллицы)
                ZipFile.ExtractToDirectory(vclFilePath, tempUnpackPath, Encoding.UTF8);

                // 2. Переносим library.json в папку CacheSession/JSONs/ИмяБиблиотеки.json
                string tempJsonPath = Path.Combine(tempUnpackPath, "library.json");
                if (File.Exists(tempJsonPath))
                {
                    string targetJsonPath = Path.Combine(CacheJsonsFolderPath, libName + ".json");
                    if (File.Exists(targetJsonPath)) File.Delete(targetJsonPath);
                    File.Move(tempJsonPath, targetJsonPath);
                }
                else
                {
                    Debug.LogWarning($"[Storage] Файл library.json не найден в библиотеке '{libName}'!");
                }

                // 3. Переносим остальные файлы (аудио и т.д.) в CacheSession/Audio/ИмяБиблиотеки/
                string targetAudioFolder = Path.Combine(CacheAudioFolderPath, libName);
                if (Directory.Exists(targetAudioFolder))
                {
                    Directory.Delete(targetAudioFolder, true);
                }
                Directory.CreateDirectory(targetAudioFolder);

                foreach (string file in Directory.GetFiles(tempUnpackPath))
                {
                    string fileName = Path.GetFileName(file);
                    File.Move(file, Path.Combine(targetAudioFolder, fileName));
                }

                // Подчищаем промежуточную папку
                Directory.Delete(tempUnpackPath, true);

                Debug.Log($"[Storage] Библиотека '{libName}' успешно распакована в кэш.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Storage] Ошибка при распаковке библиотеки '{libName}': {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Находит все JSON-файлы в папке кэша и возвращает массив объектов LibraryData.
    /// </summary>
    public static LibraryData[] GetLoadedLibrariesFromCache()
    {
        if (!Directory.Exists(CacheJsonsFolderPath))
        {
            return new LibraryData[0];
        }

        string[] jsonFiles = Directory.GetFiles(CacheJsonsFolderPath, "*.json");
        List<LibraryData> resultList = new List<LibraryData>();

        foreach (string jsonPath in jsonFiles)
        {
            try
            {
                string jsonText = File.ReadAllText(jsonPath, Encoding.UTF8);
                LibraryData libData = JsonUtility.FromJson<LibraryData>(jsonText);
                if (libData != null)
                {
                    resultList.Add(libData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Storage] Ошибка чтения JSON файла '{jsonPath}': {ex.Message}");
            }
        }

        return resultList.ToArray();
    }

    /// <summary>
    /// Сохраняет библиотеку обратно из кэша в архив .vcl в persistentDataPath.
    /// </summary>
    public static void SaveLibrary(string libraryName)
    {
        string jsonPath = Path.Combine(CacheJsonsFolderPath, libraryName + ".json");
        string audioFolderPath = Path.Combine(CacheAudioFolderPath, libraryName);
        string vclFilePath = Path.Combine(LibrariesFolderPath, libraryName + ".vcl");

        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"[Storage] Нельзя сохранить '{libraryName}': JSON файл отсутствует в кэше.");
            return;
        }

        try
        {
            // Временная папка для сборки
            string tempPackFolder = Path.Combine(Application.temporaryCachePath, "TempPack_" + libraryName);
            if (Directory.Exists(tempPackFolder))
            {
                Directory.Delete(tempPackFolder, true);
            }
            Directory.CreateDirectory(tempPackFolder);

            // Копируем JSON с именем library.json внутрь папки сборки
            File.Copy(jsonPath, Path.Combine(tempPackFolder, "library.json"), true);

            // Копируем все аудиофайлы в папку сборки
            if (Directory.Exists(audioFolderPath))
            {
                foreach (string file in Directory.GetFiles(audioFolderPath))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(tempPackFolder, fileName), true);
                }
            }

            // Создаем временный zip с поддержкой UTF-8
            string tempZipPath = Path.Combine(Application.temporaryCachePath, libraryName + "_temp.vcl");
            if (File.Exists(tempZipPath)) File.Delete(tempZipPath);

            ZipFile.CreateFromDirectory(tempPackFolder, tempZipPath, System.IO.Compression.CompressionLevel.Optimal, false, Encoding.UTF8);

            // Заменяем существующий файл .vcl
            if (File.Exists(vclFilePath))
            {
                File.Delete(vclFilePath);
            }
            File.Move(tempZipPath, vclFilePath);

            // Чистим папку сборки
            Directory.Delete(tempPackFolder, true);

            Debug.Log($"[Storage] Библиотека '{libraryName}.vcl' успешно сохранена из кэша!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Storage] Ошибка при сохранении библиотеки '{libraryName}': {ex.Message}");
        }
    }

    /// <summary>
    /// Полностью очищает временный кэш сессии.
    /// Вызывать при закрытии интерфейса теста/меню.
    /// </summary>
    public static void ClearCache()
    {
        if (Directory.Exists(CacheFolderPath))
        {
            try
            {
                Directory.Delete(CacheFolderPath, true);
                Debug.Log("[Storage] Кэш сессии успешно очищен.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Storage] Ошибка при очистке кэша: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Удаляет файл библиотеки (.vcl) из основного хранилища.
    /// </summary>
    public static void DeleteLibrary(string libraryName)
    {
        string vclFilePath = Path.Combine(LibrariesFolderPath, libraryName + ".vcl");
        if (File.Exists(vclFilePath))
        {
            File.Delete(vclFilePath);
            Debug.Log($"[Storage] Библиотека '{libraryName}.vcl' успешно удалена.");
        }
        else
        {
            Debug.LogWarning($"[Storage] Библиотека '{libraryName}.vcl' не найдена для удаления.");
        }
    }

    /// <summary>
    /// Возвращает имена всех имеющихся .vcl библиотек.
    /// </summary>
    public static List<string> GetLibraryNames()
    {
        List<string> libraryNames = new List<string>();
        if (!Directory.Exists(LibrariesFolderPath)) return libraryNames;

        string[] files = Directory.GetFiles(LibrariesFolderPath, "*.vcl");
        foreach (string file in files)
        {
            libraryNames.Add(Path.GetFileNameWithoutExtension(file));
        }

        return libraryNames;
    }

    /// <summary>
    /// Вспомогательный метод: проверяет и создаёт структуру папок кэша.
    /// </summary>
    public static void EnsureCacheDirectoriesExist()
    {
        if (!Directory.Exists(CacheJsonsFolderPath))
            Directory.CreateDirectory(CacheJsonsFolderPath);

        if (!Directory.Exists(CacheAudioFolderPath))
            Directory.CreateDirectory(CacheAudioFolderPath);
    }

    /// <summary>
    /// Преобразует объект библиотеки в JSON и записывает его в папку кэша JSONs.
    /// </summary>
    public static bool SaveJsonToCache(LibraryData library)
    {
        if (library == null || string.IsNullOrEmpty(library.LibraryName))
        {
            Debug.Log("[Storage] Невозможно сохранить JSON: объект библиотеки пуст или имя не задано!");
            return false;
        }

        try
        {
            EnsureCacheDirectoriesExist();

            // Формируем путь: CacheSession/JSONs/ИмяБиблиотеки.json
            string jsonPath = Path.Combine(CacheJsonsFolderPath, library.LibraryName + ".json");

            // Преобразуем объект в JSON текст (true - с красивыми отступами)
            string jsonText = JsonUtility.ToJson(library, true);

            // Пишем файл с UTF-8 кодировкой для поддержки кириллицы
            File.WriteAllText(jsonPath, jsonText, System.Text.Encoding.UTF8);

            Debug.Log($"[Storage] JSON для библиотеки '{library.LibraryName}' записан в кэш.");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Storage] Ошибка создания JSON в кэше: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Удаляет файл JSON из кэша по имени библиотеки.
    /// </summary>
    public static bool DeleteJsonFromCache(string libraryName)
    {
        if (string.IsNullOrEmpty(libraryName)) return false;

        try
        {
            string jsonPath = Path.Combine(CacheJsonsFolderPath, libraryName + ".json");

            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
                Debug.Log($"[Storage] Старый JSON '{libraryName}.json' удален из кэша.");
                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Storage] Ошибка удаления JSON из кэша: {ex.Message}");
            return false;
        }
    }

}