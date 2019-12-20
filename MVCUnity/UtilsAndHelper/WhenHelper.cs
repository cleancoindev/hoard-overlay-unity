using System;
using System.Collections;
using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Encapsulates the behaviour when we want to execute callback after some condition is met and we don't want to do this synchronusly
    /// </summary>
    public static class When
    {
        private static GameObject routineGameObject;
        private static MonoBehaviour routineMono;

        static When()
        {
            routineMono = GameObject.FindObjectOfType<CoroutineContext>();
            if (routineMono == null)
            {
                routineGameObject = new GameObject("RoutineHandler");
                routineGameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.NotEditable;
                routineMono = routineGameObject.AddComponent<CoroutineContext>();
            }
            else
            {
                routineGameObject = routineMono.gameObject;
            }
        }

        private static IEnumerator TrueRoutine(Func<bool> prediction, Action action, float timeOut = -1f)
        {
            if (prediction == null) yield break;
            var checkTime = timeOut > 0f;
            while (!prediction())
            {
                if (checkTime && timeOut < 0) yield break;
                yield return null;
            }
            action?.Invoke();
        }

        public static void True(Func<bool> prediction, Action action, float timeOut = -1f)
        {
            routineMono.StartCoroutine(TrueRoutine(prediction, action, timeOut));
        }
    }
}
