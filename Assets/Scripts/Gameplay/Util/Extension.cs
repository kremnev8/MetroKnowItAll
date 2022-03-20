using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Util
{
    public static class Extension
    {
        public static void Invoke(this MonoBehaviour behaviour, string method, object options, float delay)
        {
            behaviour.StartCoroutine(_invoke(behaviour, method, delay, options));
        }

        private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, float delay, object options)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            Type instance = behaviour.GetType();
            MethodInfo mthd = instance.GetMethod(method);
            mthd?.Invoke(behaviour, new object[] {options});

            yield return null;
        }

        public static void ClearChildren(this GameObject thatObject)
        {
            int i = 0;

            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[thatObject.transform.childCount];

            //Find all child obj and store to that array
            foreach (Transform child in thatObject.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            if (i == 0)
            {
                Object.Destroy(thatObject);
                return;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                child.gameObject.ClearChildren();
            }

        }
        
        public static void ChangeLayersRecursively(this Transform trans, int mask)
        {
            trans.gameObject.layer = mask;
            foreach(Transform child in trans)
            {            
                child.ChangeLayersRecursively(mask);
            }
        }
        
        
        public static void ExecuteInAllChildren(this Transform trans, Action<Transform> action, bool executeOnMain = false)
        {
            if (executeOnMain)
                action(trans);
            
            foreach(Transform child in trans)
            {            
                child.ExecuteInAllChildren(action, true);
            }
        }

        public static Vector2 ClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3) {

             float xDelta = p2.x - p1.x;
             float yDelta = p2.y - p1.y;

            if ((xDelta == 0) && (yDelta == 0))
            {
                return p3;
            }

            float u = ((p3.x - p1.x) * xDelta + (p3.y - p1.y) * yDelta) / (xDelta * xDelta + yDelta * yDelta);

            return new Vector2(p1.x + u * xDelta, p1.y + u * yDelta);
        }

        public static Vector3 ClampHorizontal(this Vector3 vector, float maxMagnitude)
        {
            float y = vector.y;
            vector.y = 0;
            if (vector.sqrMagnitude > maxMagnitude * maxMagnitude)
            {
                vector = vector.normalized * maxMagnitude;
            }
            
            vector.y = y;
            return vector;
        }
        
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
		
        public static Vector2 CopyVector(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static System.Numerics.Vector2 Convert(this Vector2 vector)
        {
            return new System.Numerics.Vector2(vector.x, vector.y);
        }
		
        public static Vector2 Convert(this System.Numerics.Vector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
		
        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.x,0, vector.y);
        }
		
        public static Vector2 ToVector2(this Vector3Int vector)
        {
            return new Vector2(vector.x, vector.z);
        }
		
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static bool Less(this Vector3 vec1, Vector3 vec2)
        {
            return vec1.x <= vec2.x && vec1.y <= vec2.y && vec1.z <= vec2.z;
        }
		
        public static bool Greater(this Vector3 vec1, Vector3 vec2)
        {
            return vec1.x >= vec2.x && vec1.y >= vec2.y && vec1.z >= vec2.z;
        }

        public static Vector3 GetBiggestAxis(this Vector3 vector)
        {
            float x = Mathf.Abs(vector.x);
            float y = Mathf.Abs(vector.y);
            float z = Mathf.Abs(vector.z);
            
            if (x > y && x > z)
            {
                return new Vector3(vector.x, 0, 0);
            }
            
            if (y > x && y > z)
            {
                return new Vector3(0, vector.y, 0);
            }
            
            if (z > x && z > y)
            {
                return new Vector3(0, 0, vector.z);
            }
            return Vector3.zero;
        }

        public static RigidbodyConstraints GetConstaraints(this Vector3 vector)
        {
            int value = vector.x == 0 ? (int)RigidbodyConstraints.FreezePositionX : 0;
            value += vector.y == 0 ? (int)RigidbodyConstraints.FreezePositionY : 0;
            value += vector.z == 0 ? (int)RigidbodyConstraints.FreezePositionZ : 0;

            return (RigidbodyConstraints)value;
        }
    }
}