using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

public class TimelineBinding : MonoBehaviour
{
    private PlayableDirector director;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        BindAllTracks();
    }

    void BindAllTracks()
    {
        foreach (var output in director.playableAsset.outputs)
        {
            string trackName = output.streamName;

            // 已绑定的跳过
            if (director.GetGenericBinding(output.sourceObject) != null)
                continue;

            // 查找物体（从场景根开始，支持子物体）
            GameObject target = FindGameObjectInScene(trackName);
            if (target == null)
            {
                continue;
            }

            director.SetGenericBinding(output.sourceObject, target);
        }
    }

    /// <summary>
    /// 从场景根开始查找物体（支持路径和名称）
    /// </summary>
    GameObject FindGameObjectInScene(string nameOrPath)
    {
        // 方法1：按路径查找（从场景根）
        if (nameOrPath.Contains("/"))
        {
            GameObject root = GameObject.Find(nameOrPath.Split('/')[0]);
            if (root != null)
            {
                Transform t = root.transform.Find(nameOrPath.Substring(nameOrPath.IndexOf('/') + 1));
                if (t != null) return t.gameObject;
            }
        }

        // 方法2：按名称递归查找场景中所有物体
        return FindChildInScene(null, nameOrPath);
    }

    /// <summary>
    /// 递归查找场景中的子物体
    /// </summary>
    GameObject FindChildInScene(GameObject parent, string name)
    {
        GameObject[] allObjects;

        if (parent == null)
        {
            // 从场景根开始：查找所有根物体
            allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }
        else
        {
            // 从父物体开始：获取所有直接子物体
            allObjects = new GameObject[parent.transform.childCount];
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                allObjects[i] = parent.transform.GetChild(i).gameObject;
            }
        }

        foreach (var obj in allObjects)
        {
            if (obj.name == name)
                return obj;

            GameObject result = FindChildInScene(obj, name);
            if (result != null)
                return result;
        }

        return null;
    }

    string GetPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }

    public void Rebind()
    {
        BindAllTracks();
    }
}