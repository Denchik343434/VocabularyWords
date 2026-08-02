using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression; 

// TODO: добавить сохранение аудио файлов в zip архиве

// скрипт с статичными методами для использования в компонентах, для работы с файлами
public class StorageManager
{
    //Путь к папке с библиотеками
    public static string LibrariesFolderPath => Path.Combine(Application.persistentDataPath, "Libraries");

    // Сохранить библиотеку в zip - .vcl с json 
    public static void SaveLibrary(LibraryData library)
    {

        string tempFolder = Path.Combine(Application.temporaryCachePath, library.LibraryName);
        string vclFilePath = Path.Combine(LibrariesFolderPath, library.LibraryName + ".vcl");

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
    public static LibraryData LoadLibrary(string libraryName)
    {
        string vclFilePath = Path.Combine(LibrariesFolderPath, libraryName + ".vcl");

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
}