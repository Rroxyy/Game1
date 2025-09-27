using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class ResourceManager
{
    private static readonly Dictionary<string, AsyncOperationHandle> loadedAssets = new();

    /// <summary>
    /// 同步加载（阻塞式）
    /// </summary>
    public static T LoadSync<T>(string address) where T : UnityEngine.Object
    {
        if (loadedAssets.ContainsKey(address))
            return (T)loadedAssets[address].Result;

        var handle = Addressables.LoadAssetAsync<T>(address);
        handle.WaitForCompletion(); // 阻塞直到完成
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedAssets[address] = handle;
            return handle.Result;
        }

        Debug.LogError($"LoadSync 失败: {address}");
        return null;
    }

    /// <summary>
    /// 异步加载（Task）
    /// </summary>
    public static async Task<T> LoadAsync<T>(string address) where T : UnityEngine.Object
    {
        if (loadedAssets.ContainsKey(address))
            return (T)loadedAssets[address].Result;

        var handle = Addressables.LoadAssetAsync<T>(address);
        loadedAssets[address] = handle;

        try
        {
            var result = await handle.Task;
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadAsync 失败: {address}, 错误: {e}");
            return null;
        }
    }

    /// <summary>
    /// 异步加载（回调）
    /// </summary>
    public static void LoadAsync<T>(string address, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (loadedAssets.ContainsKey(address))
        {
            onLoaded?.Invoke((T)loadedAssets[address].Result);
            return;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        loadedAssets[address] = handle;

        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"LoadAsync 回调失败: {address}");
                onLoaded?.Invoke(null);
            }
        };
    }

    /// <summary>
    /// 卸载单个资源
    /// </summary>
    public static void Unload(string address)
    {
        if (loadedAssets.TryGetValue(address, out var handle))
        {
            Addressables.Release(handle);
            loadedAssets.Remove(address);
        }
    }

    /// <summary>
    /// 卸载所有已加载资源
    /// </summary>
    public static void UnloadAll()
    {
        foreach (var kvp in loadedAssets)
        {
            Addressables.Release(kvp.Value);
        }
        loadedAssets.Clear();
    }

    /// <summary>
    /// 查询资源是否已经加载
    /// </summary>
    public static bool IsLoaded(string address)
    {
        return loadedAssets.ContainsKey(address);
    }
}
