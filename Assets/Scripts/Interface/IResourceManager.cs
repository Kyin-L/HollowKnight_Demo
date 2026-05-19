using UnityEngine;

public interface IResourceManager
{
    public T Load<T>(string path) where T : Object;

    public void LoadAsync<T>(System.Action<T> callback, params string[] strs) where T : Object;

    public void UnloadUnusedAssets(System.Action callback = null);
}
