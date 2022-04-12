using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Util
{
    public static class GameObjectExtensions
    {
        public static void Invoke(this MonoBehaviour behaviour, string method, object options, float delay)
        {
            behaviour.StartCoroutine((IEnumerator) _invoke(behaviour, method, delay, options));
        }

        private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, float delay, object options)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            Type instance = behaviour.GetType();
            MethodInfo mthd = instance.GetMethod(method);
            mthd?.Invoke(behaviour, new[] {options});

            yield return null;
        }

        public static void ClearChildren(this GameObject thatObject)
        {
            //Array to hold all child obj
            List<GameObject> allChildren = new List<GameObject>(thatObject.transform.childCount);

            //Find all child obj and store to that array
            foreach (Transform child in thatObject.transform)
            {
                allChildren.Add(child.gameObject);
            }

            if (allChildren.Count == 0) return;


            //Now destroy them
            foreach (GameObject child in allChildren)
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(child);
#else
                Object.Destroy(child);
#endif
            }
        }

        public static void ChangeLayersRecursively(this Transform trans, int mask)
        {
            trans.gameObject.layer = mask;
            foreach (Transform child in trans) child.ChangeLayersRecursively(mask);
        }

        public static void ExecuteInAllChildren(this Transform trans, Action<Transform> action, bool executeOnMain = false)
        {
            if (executeOnMain)
                action(trans);

            foreach (Transform child in trans) child.ExecuteInAllChildren(action, true);
        }

        public static RigidbodyConstraints GetConstaraints(this Vector3 vector)
        {
            int value = vector.x == 0 ? (int) RigidbodyConstraints.FreezePositionX : 0;
            value += vector.y == 0 ? (int) RigidbodyConstraints.FreezePositionY : 0;
            value += vector.z == 0 ? (int) RigidbodyConstraints.FreezePositionZ : 0;

            return (RigidbodyConstraints) value;
        }
    }
}