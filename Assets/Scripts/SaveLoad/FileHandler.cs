using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Rendering;
using System.Linq;

// jsonUtility is the package used for converting from unity objects to json and vice versa
// you need to wrap your objects in key value pairs so it can work
public static class FileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string fileName)
    {
        Debug.Log($"getting path->{GetPath(fileName)}");
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteToFile(GetPath(fileName), content);
    }
    public static List<T> ReadFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));
        if (string.IsNullOrEmpty(content))
        {
            // retunring an empty string
            return new List<T>();
        }
        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }
    private static string GetPath(string fileName)
    {
        // works for all devices that Unity supports
        return Application.persistentDataPath + "/" + fileName;
    }
    private static void WriteToFile(string path, string content)
    {
        // create a new fileStream
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }
    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}