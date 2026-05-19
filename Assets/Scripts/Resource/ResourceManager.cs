using System.Collections;
using UnityEngine;

public class ResourceManager : SingletonBase<ResourceManager>, IResourceManager
{
    private IMonoManager m_monoManager;

    private ResourceManager()
    {
        m_monoManager = ManagerLocator.Get<IMonoManager>();
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public void LoadAsync<T>(System.Action<T> callback, params string[] strs) where T : Object
    {
        string path = Tools.Path.GetPath(strs);
        m_monoManager.StartCoroutine(IELoadAsync(path, callback));
    }

    private IEnumerator IELoadAsync<T>(string path, System.Action<T> callback) where T : Object
    {
        ResourceRequest req = Resources.LoadAsync<T>(path);
        yield return req;
        callback(req.asset as T);
    }

    public void UnloadUnusedAssets(System.Action callback = null)
    {
        m_monoManager.StartCoroutine(IEUnloadUnusedAssets(callback));
    }

    private IEnumerator IEUnloadUnusedAssets(System.Action callback)
    {
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callback?.Invoke();
    }
}