using UnityEngine;

namespace UnityUtils.ScriptUtils.ScriptableObjects
{
    public static class ScriptableObjectManager
    {
        /// <summary>
        /// Gets an array of <see cref="ScriptableObject"/> found in [Resources/'path']. Make sure you have a 'Resources' folder created
        /// </summary>
        /// <param name="path">file path to where <see cref="ScriptableObject"/> are held (found in [Resources/'path']</param>
        /// <returns>Array of specified scriptable object type</returns>
        public static T[] GetScriptableObjects<T>(string path) where T : ScriptableObject
        {
            T[] loadedObjects = Resources.LoadAll<T>(path);

            if (loadedObjects.Length <= 0)
            {
                Debug.LogWarning("No commands found in Resources/" + path + " folder.");
                return default;
            }

            foreach (T obj in loadedObjects)
            {
                Debug.Log("Loaded ScriptableObject: " + obj.name);
            }

            return loadedObjects;
        }
    }
}