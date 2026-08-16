using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;
public class AudioManager
{
    public static event Action onFinished;
    private static AudioSource _source;
    private static GameObject _manager;
    private static readonly Dictionary<string, AudioClip> _clips = new();

    private static CancellationTokenSource _playbackCts;

    public static void EnsureSource()
    {
        _manager = new GameObject("AudioManager");
        UnityEngine.Object.DontDestroyOnLoad(_manager);
        _source = _manager.AddComponent<AudioSource>();
        Debug.Log("Аудио сервер создан");
    }

    public static async Task RefreshAudioDictionaryAsync()
    {
        UnloadCurrentLibrary();
        _clips.Clear();
        var loadedClips = await StorageManager.LoadAudioClipsFromCacheAsync();
        foreach (var pair in loadedClips)
        {
            _clips[pair.Key] = pair.Value;
        }
        Debug.Log("Должно было распокаваться");
        //await Task.Delay(3000);
    }


    /// <summary>
    /// Добавляет один файл в кэш и сразу же загружает его в оперативку.
    /// </summary>
    /// <param name="clipName">Имя для клипа (без расширения)</param>
    /// <param name="sourceFilePath">Исходный путь к файлу на ПК</param>
    public static async Task AddAudioClipAsync(string clipName, string sourceFilePath, CancellationToken token = default)
    {
            // 1. Копируем файл в кэш
            string cachedPath = StorageManager.ClipAudio(clipName, sourceFilePath);
            if (string.IsNullOrEmpty(cachedPath))
            {
                Debug.LogError($"[AudioManager] Не удалось скопировать файл '{sourceFilePath}' в кэш.");
                return;
            }

            // 2. Если клип с таким именем уже был в словаре — выгружаем его из памяти
            if (_clips.TryGetValue(clipName, out AudioClip oldClip))
            {
                if (oldClip != null) UnityEngine.Object.Destroy(oldClip);
            }

            // 3. Загружаем только один этот файл в RAM
            using var request = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip("file://" + cachedPath, AudioType.UNKNOWN);
            var op = request.SendWebRequest();

            while (!op.isDone)
            {
                if (token.IsCancellationRequested)
                {
                    request.Abort();
                    return;
                }
                await Task.Yield();
            }

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(request);
                if (clip != null)
                {
                    clip.name = clipName;
                    _clips[clipName] = clip;
                    Debug.Log($"[AudioManager] Клип '{clipName}' успешно добавлен в оперативку и кэш.");
                    return;
                }
            }

            Debug.LogError($"[AudioManager] Ошибка загрузки клипа '{clipName}': {request.error}");
    }

    /// <summary>
    /// Воспроизводит аудио по имени файла (без расширения).
    /// </summary>
    /// <summary>
    /// Воспроизводит клип из оперативной памяти по названию файла.
    /// </summary>
    public static async void Play(string fileName)
    {   
        // Отменяем прошлый трек БЕЗ вызова onFinished
        StopInternal();

        if (!_clips.TryGetValue(fileName, out AudioClip clip))
        {
            Debug.LogWarning($"[AudioManager] Клип '{fileName}' не найден!");
            onFinished?.Invoke();
            return;
        }

        _source.clip = clip;
        _source.Play();

        _playbackCts = new CancellationTokenSource();
        var token = _playbackCts.Token;

        try
        {
            while (_source != null && _source.isPlaying)
            {
                await Task.Yield();
                if (token.IsCancellationRequested) return;
            }

            // Звук доиграл сам
            if (!token.IsCancellationRequested)
            {
                onFinished?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AudioManager] Ошибка в отслеживании клипа: {ex.Message}");
        }
    }
    public static void Stop()
    {
        StopInternal();
        onFinished?.Invoke();
    }

    private static void StopInternal()
    {
        if (_playbackCts != null)
        {
            _playbackCts.Cancel();
            _playbackCts.Dispose();
            _playbackCts = null;
        }

        if (_source != null && _source.isPlaying)
        {
            _source.Stop();
        }
    }

    /// <summary>
    /// Полностью очищает оперативку от текущих аудиоклипов.
    /// </summary>
    public static void UnloadCurrentLibrary()
    {
        Stop();

        foreach (AudioClip clip in _clips.Values)
        {
            if (clip != null)
                UnityEngine.Object.Destroy(clip);
        }

        _clips.Clear();
    }

    /// <summary>
    /// Удаляет клип из оперативной памяти и стирает файл из кэша на диске.
    /// </summary>
    /// <param name="clipName">Имя файла/клипа без расширения</param>
    public static void DeleteAudioClip(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return;

        // 1. Если сейчас играет именно этот клип — останавливаем проигрывание
        if (_source != null && _source.clip != null && _source.clip.name == clipName)
        {
            Stop();
        }

        // 2. Выгружаем из оперативки Unity и удаляем из словаря
        if (_clips.TryGetValue(clipName, out AudioClip clip))
        {
            if (clip != null)
            {
                UnityEngine.Object.Destroy(clip);
            }
            _clips.Remove(clipName);
        }

        // 3. Удаляем файл с диска
        StorageManager.DeleteAudioFile(clipName);

        Debug.Log($"[AudioManager] Клип '{clipName}' успешно удален из памяти и кэша.");
    }

    /// <summary>
    /// Переименовывает аудиоклип в оперативной памяти и обновляет имя файла на диске.
    /// </summary>
    /// <param name="oldName">Старое имя клипа (без расширения)</param>
    /// <param name="newName">Новое имя клипа (без расширения)</param>
    public static bool RenameAudioClip(string oldName, string newName)
    {
        if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
            return true;

        // 1. Переименовываем файл в кэше на диске
        StorageManager.RenameAudioFile(oldName, newName);

        // 2. Если клип загружен в оперативку — обновляем имя самого объекта и ключ словаря
        if (_clips.TryGetValue(oldName, out AudioClip clip))
        {
            if (clip != null)
            {
                clip.name = newName;
                _clips[newName] = clip;
            }
            _clips.Remove(oldName);

            Debug.Log($"[AudioManager] Клип в RAM переименован с '{oldName}' на '{newName}'.");
        }

        return true;
    }
}
