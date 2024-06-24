using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utility
{
    public static class UnityHelpers
    {
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static void ClearAndDestroyObjects(this List<GameObject> list)
        {
            if (list == null) return;

            foreach (GameObject obj in list)
            {
                MonoBehaviour.Destroy(obj);
            }

            list.Clear();
        }

        public static void RemoveCloneNameSuffix(this GameObject gameObject)
        {
            gameObject.name = gameObject.name.Split(new string[] { "(Clone)" }, StringSplitOptions.None)[0];
        }
    }
}