using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Networking;
using SFB;

// скрипт с статичными методами для использования в компонентах, для работы с файлами
public class StorageManager
{
    // Путь к постоянному хранилищу с .vcl архивами
    private static string _librariesFolderPath => Path.Combine(Application.persistentDataPath, "Libraries");

    // Корневая рабочая папка в кэше для текущей сессии
    private static string _cacheFolderPath => Path.Combine(Application.temporaryCachePath, "CacheSession");

    // Внутренние подпапки в кэше для порядка
    private static string _cacheJsonsFolderPath => Path.Combine(_cacheFolderPath, "JSONs");
    private static string _cacheAudioFolderPath => Path.Combine(_cacheFolderPath, "Audio");

    /// <summary>
    /// Инициализация хранилища: создаёт папку с библиотеками и чистит старый кэш при запуске.
    /// </summary>
    public static void InitializeStorage()
    {
        if (!Directory.Exists(_librariesFolderPath))
        {
            Directory.CreateDirectory(_librariesFolderPath);
        }
    }

    /// <summary>
    /// Распаковывает список библиотек по их именам во временный кэш.
    /// </summary>
    public static async Task UnpackLibrariesToCache(params string[] libraryNames)
    {
        string librariesFolderPath = _librariesFolderPath;
        string tempCachePath = Application.temporaryCachePath;
        string cacheJsonsFolderPath = _cacheJsonsFolderPath;
        string cacheAudioFolderPath = _cacheAudioFolderPath;

        EnsureCacheDirectoriesExist();

        await Task.Run(() =>
        {

            foreach (string libName in libraryNames)
            {
                string vclFilePath = Path.Combine(librariesFolderPath, libName + ".vcl");

                if (!File.Exists(vclFilePath))
                {
                    continue;
                }

                try
                {
                    // Временная папка для промежуточной распаковки архива
                    string tempUnpackPath = Path.Combine(tempCachePath, "TempUnpack_" + libName);
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
                        string targetJsonPath = Path.Combine(cacheJsonsFolderPath, libName + ".json");
                        if (File.Exists(targetJsonPath)) File.Delete(targetJsonPath);
                        File.Move(tempJsonPath, targetJsonPath);
                    }

                    // 3. Переносим остальные файлы (аудио и т.д.) в CacheSession/Audio/

                    foreach (string file in Directory.GetFiles(tempUnpackPath))
                    {
                        string fileName = Path.GetFileName(file);
                        string destPath = Path.Combine(cacheAudioFolderPath, fileName);

                        // Перезаписываем файл, если он уже существует
                        File.Copy(file, destPath, true);
                        File.Delete(file);
                    }

                    // Подчищаем промежуточную папку
                    Directory.Delete(tempUnpackPath, true);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        });
    }

    /// <summary>
    /// Находит все JSON-файлы в папке кэша и возвращает массив объектов LibraryData.
    /// </summary>
    public static LibraryData[] GetLoadedLibrariesFromCache()
    {
        if (!Directory.Exists(_cacheJsonsFolderPath))
        {
            return new LibraryData[0];
        }

        string[] jsonFiles = Directory.GetFiles(_cacheJsonsFolderPath, "*.json");
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
                Debug.Log(ex);
            }
        }

        return resultList.ToArray();
    }

    /// <summary>
    /// Сохраняет библиотеку обратно из кэша в архив .vcl в persistentDataPath.
    /// </summary>
    
    public static async Task SaveLibraryAsync(string saveFolderPath = null)
    {
        saveFolderPath ??= _librariesFolderPath;
        string libraryName = GetLoadedLibrariesFromCache()[0].LibraryName;
        string jsonPath = Path.Combine(_cacheJsonsFolderPath, libraryName + ".json");
        string vclFilePath = Path.Combine(saveFolderPath, libraryName + ".vcl");
        string tempCacheFolder = Application.temporaryCachePath;
        string cacheAudioFolderPath = _cacheAudioFolderPath;

        if (!File.Exists(jsonPath))
        {
            return;
        }

        await Task.Run(() => {
            // Временная папка для сборки
            string tempPackFolder = Path.Combine(tempCacheFolder, "TempPack_" + libraryName);
            if (Directory.Exists(tempPackFolder))
            {
                Directory.Delete(tempPackFolder, true);
            }
            Directory.CreateDirectory(tempPackFolder);

            // Копируем JSON с именем library.json внутрь папки сборки
            File.Copy(jsonPath, Path.Combine(tempPackFolder, "library.json"), true);

            // Копируем все аудиофайлы в папку сборки
            if (Directory.Exists(cacheAudioFolderPath))
            {
                foreach (string file in Directory.GetFiles(cacheAudioFolderPath))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(tempPackFolder, fileName), true);
                }
            }

            // Создаем временный zip с поддержкой UTF-8
            string tempZipPath = Path.Combine(tempCacheFolder, libraryName + "_temp.vcl");
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

        });
        //await Task.Delay(3000);

    }

    /// <summary>
    /// Полностью очищает временный кэш сессии.
    /// Вызывать при закрытии интерфейса теста/меню.
    /// </summary>
    public static void ClearCache()
    {
        if (Directory.Exists(_cacheFolderPath))
        {
            try
            {
                Directory.Delete(_cacheFolderPath, true);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    /// <summary>
    /// Удаляет файл библиотеки (.vcl) из основного хранилища.
    /// </summary>
    public static void DeleteLibrary(string libraryName)
    {
        string vclFilePath = Path.Combine(_librariesFolderPath, libraryName + ".vcl");
        if (File.Exists(vclFilePath))
        {
            File.Delete(vclFilePath);
        }
        else
        {
        }
    }

    /// <summary>
    /// Возвращает имена всех имеющихся .vcl библиотек.
    /// </summary>
    public static List<string> GetLibraryNames()
    {
        List<string> libraryNames = new List<string>();
        if (!Directory.Exists(_librariesFolderPath)) return libraryNames;

        string[] files = Directory.GetFiles(_librariesFolderPath, "*.vcl");
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
        if (!Directory.Exists(_cacheJsonsFolderPath))
            Directory.CreateDirectory(_cacheJsonsFolderPath);

        if (!Directory.Exists(_cacheAudioFolderPath))
            Directory.CreateDirectory(_cacheAudioFolderPath);
    }

    /// <summary>
    /// Преобразует объект библиотеки в JSON и записывает его в папку кэша JSONs.
    /// </summary>
    public static bool SaveJsonToCache(LibraryData library)
    {
        if (library == null || string.IsNullOrEmpty(library.LibraryName))
        {
            return false;
        }

        try
        {
            EnsureCacheDirectoriesExist();

            // Формируем путь: CacheSession/JSONs/ИмяБиблиотеки.json
            string jsonPath = Path.Combine(_cacheJsonsFolderPath, library.LibraryName + ".json");

            // Преобразуем объект в JSON текст (true - с красивыми отступами)
            string jsonText = JsonUtility.ToJson(library, true);

            // Пишем файл с UTF-8 кодировкой для поддержки кириллицы
            File.WriteAllText(jsonPath, jsonText, System.Text.Encoding.UTF8);

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
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
            string jsonPath = Path.Combine(_cacheJsonsFolderPath, libraryName + ".json");

            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
    }

    /// <summary>
    /// Вызывает системный проводник. Возвращает выбранный путь или null, если пользователь закрыл окно.
    /// </summary>
    /// <param name="filterType">Что нужно выбрать: StorageFilterType.Archive, Audio, Folder или AnyFile</param>
    /// <param name="title">Заголовок окна (необязательно)</param>
    public static string GetUserPath(StorageFilterType filterType)
    {
        try
        {
            string title = "ошибка названия";

            // 1. Если требуется выбрать именно ПАПКУ
            if (filterType == StorageFilterType.Folder)
            {
                title = "Выберете папку";
                string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel(title, "", multiselect: false);
                //string[] folderPaths = StandaloneFileBrowser.OpenFolderPanel(title, "", multiselect: false);
                return (folderPaths != null && folderPaths.Length > 0 && !string.IsNullOrEmpty(folderPaths[0])) 
                    ? folderPaths[0] 
                    : null;
            }

            // 2. Настраиваем фильтрацию для ФАЙЛОВ
            ExtensionFilter[] extensions;

            switch (filterType)
            {
                case StorageFilterType.Library:
                    title = "Выберете файл библиотеки";
                    extensions = new[] {
                        new ExtensionFilter("Архивы библиотеки", "vcl"),
                        new ExtensionFilter("Все файлы", "*")
                    };
                    break;

                case StorageFilterType.Audio:
                    title = "Выберете аудио файл";
                    extensions = new[] {
                        new ExtensionFilter("Аудиофайлы", "mp3", "wav", "ogg"),
                        new ExtensionFilter("Все файлы", "*")
                    };
                    break;

                default: // AnyFile
                    title = "Выберете файл";
                    extensions = new[] {
                        new ExtensionFilter("Все файлы", "*")
                    };
                    break;
            }

            // 3. Открываем проводник для файлов
            string[] filePaths = StandaloneFileBrowser.OpenFilePanel(title, "", extensions, multiselect: false);
            //string[] filePaths = StandaloneFileBrowser.OpenFilePanel(title, "", extensions, multiselect: false);

            if (filePaths != null && filePaths.Length > 0 && !string.IsNullOrEmpty(filePaths[0]))
            {
                return filePaths[0];
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        return null;
    }



    /// <summary>
    /// Копирует файл библиотеки (.vcl) из указанного пути в основное хранилище библиотек.
    /// Если файл с таким именем уже есть — перезаписывает.
    /// </summary>
    public static void AddLibrary(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            if (!Directory.Exists(_librariesFolderPath))
                Directory.CreateDirectory(_librariesFolderPath);

            string fileName = Path.GetFileName(path);
            string destPath = Path.Combine(_librariesFolderPath, fileName);

            File.Copy(path, destPath, true);

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    
    /// <summary>
    /// Копирует аудиофайл по указанному пути в папку кэша аудио.
    /// Переименовывает файл согласно переданному имени, сохраняя исходное расширение.
    /// Если в кэше загружена библиотека — файл кладётся в её подпапку.
    /// Возвращает полный путь к скопированному файлу в кэше или null при ошибке.
    /// </summary>
    public static string ClipAudio(string name, string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            EnsureCacheDirectoriesExist();
            DeleteAudioFile(name);

            string ext = Path.GetExtension(path);
            string fileName = name;
            if (!name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                fileName += ext;

            // Сохраняем напрямую в _cacheAudioFolderPath без подпапок
            string destPath = Path.Combine(_cacheAudioFolderPath, fileName);

            File.Copy(path, destPath, true);

            return destPath;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
    }

    /// <summary>
    /// Удаляет аудиофайл по его имени (без расширения) из указанной библиотеки или из всего кэша.
    /// </summary>
    public static void DeleteAudioFile(string fileNameWithoutExt)
    {
        string[] files = Directory.GetFiles(_cacheAudioFolderPath);
            foreach (string file in files)
            {
                if (Path.GetFileNameWithoutExtension(file).Equals(fileNameWithoutExt, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                }
            }
    }

    /// <summary>
    /// Переименовывает аудиофайл с oldName на newName (без расширения) с сохранением расширения.
    /// </summary>
    public static void RenameAudioFile(string oldNameWithoutExt, string newNameWithoutExt)
    {
        string[] files = Directory.GetFiles(_cacheAudioFolderPath);
        foreach (string file in files)
        {
            if (Path.GetFileNameWithoutExtension(file).Equals(oldNameWithoutExt, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string ext = Path.GetExtension(file);
                    string newFilePath = Path.Combine(_cacheAudioFolderPath, newNameWithoutExt + ext);

                    if (File.Exists(newFilePath))
                    {
                        File.Delete(newFilePath);
                    }

                    File.Move(file, newFilePath);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }
    }

    /// <summary>
    /// Асинхронно считывает все аудиофайлы из кэша и возвращает словарь [ИмяБезРасширения, AudioClip].
    /// </summary>
    public static async Task<Dictionary<string, AudioClip>> LoadAudioClipsFromCacheAsync(CancellationToken token = default)
    {
        var audioClips = new Dictionary<string, AudioClip>();

        if (!Directory.Exists(_cacheAudioFolderPath))
        {
            return audioClips;
        }

        string[] files = Directory.GetFiles(_cacheAudioFolderPath);

        foreach (string filePath in files)
        {
            if (token.IsCancellationRequested) break;

            string clipName = Path.GetFileNameWithoutExtension(filePath);

            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.UNKNOWN))
            {
                UnityWebRequestAsyncOperation op = request.SendWebRequest();
                while (!op.isDone)
                {
                    if (token.IsCancellationRequested)
                    {
                        request.Abort();
                        token.ThrowIfCancellationRequested();
                    }
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                    if (clip != null)
                    {
                        clip.name = clipName;
                        audioClips[clipName] = clip;
                    }
                }
            }
        }
        return audioClips;
    }

    public static void ClearLibraryData()
    {
        ClearCache();
        AudioManager.UnloadCurrentLibrary();
    }
}

public enum StorageFilterType
{
    Library,  // Выбор архивов (.vcl, .zip)
    Audio,    // Выбор звуков (.mp3, .wav, .ogg и т.д.)
    Folder,   // Выбор ПАПКИ
    AnyFile   // Выбор вообще любого файла
}