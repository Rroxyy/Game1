using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public static class ResourceManager
{
    // 缓存已加载的 AssetBundle
    private static Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 同步加载资源（从指定的 AB 包路径中）
    /// </summary>
    public static T LoadAsset<T>(string bundlePath, string assetName) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(bundlePath))
        {
            Debug.LogError("❌ bundlePath 为空！");
            return null;
        }

        AssetBundle bundle;
        if (!loadedBundles.TryGetValue(bundlePath, out bundle))
        {
            bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                Debug.LogError($"❌ 无法加载 AB 包: {bundlePath}");
                return null;
            }
            loadedBundles[bundlePath] = bundle;
        }

        T asset = bundle.LoadAsset<T>(assetName);
        if (asset == null)
            Debug.LogError($"⚠️ 未在 {bundlePath} 中找到资源: {assetName}");

        Debug.Log($"Loaded asset: {asset}");
        return asset;
    }

    /// <summary>
    /// 异步加载资源（带回调）
    /// </summary>
    public static void LoadAssetAsync<T>(string bundlePath, string assetName, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(bundlePath))
        {
            Debug.LogError("❌ bundlePath 为空！");
            onLoaded?.Invoke(null);
            return;
        }

        // 启动协程加载
        CoroutineRunner.Instance.StartCoroutine(LoadAssetAsyncRoutine(bundlePath, assetName, onLoaded));
    }

    private static IEnumerator LoadAssetAsyncRoutine<T>(string bundlePath, string assetName, Action<T> onLoaded) where T : UnityEngine.Object
    {
        AssetBundle bundle;
        if (!loadedBundles.TryGetValue(bundlePath, out bundle))
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundleRequest;

            bundle = bundleRequest.assetBundle;
            if (bundle == null)
            {
                Debug.LogError($"❌ 无法加载 AB 包: {bundlePath}");
                onLoaded?.Invoke(null);
                yield break;
            }

            loadedBundles[bundlePath] = bundle;
        }

        var assetRequest = bundle.LoadAssetAsync<T>(assetName);
        yield return assetRequest;

        T asset = assetRequest.asset as T;
        if (asset == null)
            Debug.LogError($"⚠️ 未在 {bundlePath} 中找到资源: {assetName}");

        onLoaded?.Invoke(asset);
    }

    /// <summary>
    /// 卸载指定 AB 包
    /// </summary>
    public static void UnloadBundle(string bundlePath, bool unloadAllLoadedObjects = false)
    {
        if (loadedBundles.TryGetValue(bundlePath, out var bundle))
        {
            bundle.Unload(unloadAllLoadedObjects);
            loadedBundles.Remove(bundlePath);
            Debug.Log($"🧹 卸载 AB 包: {bundlePath}");
        }
    }

    /// <summary>
    /// 卸载所有已加载的 AB 包
    /// </summary>
    public static void UnloadAll(bool unloadAllLoadedObjects = false)
    {
        foreach (var kv in loadedBundles)
        {
            kv.Value.Unload(unloadAllLoadedObjects);
        }
        loadedBundles.Clear();
        Debug.Log("🧹 已卸载所有 AB 包");
    }
}

/// <summary>
/// 辅助协程运行器（异步加载用）
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[CoroutineRunner]");
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }
}
