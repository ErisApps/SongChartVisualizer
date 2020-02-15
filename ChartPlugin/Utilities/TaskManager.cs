/// TaskManager.cs
/// Copyright (c) 2011, Ken Rockot  <k-e-n-@-REMOVE-CAPS-AND-HYPHENS-oz.gs>.  All rights reserved.
/// Everyone is granted non-exclusive license to do anything at all with this code.
///
/// This is a new coroutine interface for Unity.
///
/// The motivation for this is twofold:
///
/// 1. The existing coroutine API provides no means of stopping specific
///    coroutines; StopCoroutine only takes a string argument, and it stops
///    all coroutines started with that same string; there is no way to stop
///    coroutines which were started directly from an enumerator.  This is
///    not robust enough and is also probably pretty inefficient.
///
/// 2. StartCoroutine and friends are MonoBehaviour methods.  This means
///    that in order to start a coroutine, a user typically must have some
///    component reference handy.  There are legitimate cases where such a
///    constraint is inconvenient.  This implementation hides that
///    constraint from the user.
///
/// Example usage:
///
/// ----------------------------------------------------------------------------
/// IEnumerator MyAwesomeTask(UnityTask self)
/// {
///     while(true) {
///         Debug.Log("Logcat iz in ur consolez, spammin u wif messagez.");
///         yield return null;
////    }
/// }
///
/// IEnumerator TaskKiller(UnityTask self, float delay, Task t)
/// {
///     yield return new WaitForSeconds(delay);
///     t.Stop();
/// }
///
/// void SomeCodeThatCouldBeAnywhereInTheUniverse()
/// {
///     UnityTask spam = UnityTask.CreateUnityTask(MyAwesomeTask);
///     UnityTask.CreateUnityTask(TaskKiller, 5, spam);
/// }
/// ----------------------------------------------------------------------------
///
/// When SomeCodeThatCouldBeAnywhereInTheUniverse is called, the debug console
/// will be spammed with annoying messages for 5 seconds.
///
/// Simple, really.  There is no need to initialize or even refer to TaskManager.
/// When the first Task is created in an application, a "TaskManager" GameObject
/// will automatically be added to the scene root with the TaskManager component
/// attached.  This component will be responsible for dispatching all coroutines
/// behind the scenes.
///
/// Task also provides an event that is triggered when the coroutine exits.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
/// It is an error to attempt to start a task that has been stopped or which has
/// naturally terminated.
namespace SongChartVisualizer.Utilities
{
    public class UnityTask
    {
        /// Delegate for termination subscribers.  manual is true if and only if
        /// the coroutine was stopped with an explicit call to Stop().
        public delegate void PausedHandler(UnityTask self);
        public delegate void ResumedHandler(UnityTask self);
        public delegate void FinishedHandler(bool manual, UnityTask self);

        private UnityTaskManager.UnityTaskState task;

        /// Creates a new Task object for the given coroutine.
        ///
        /// If autoStart is true (default) the task is automatically started
        /// upon construction.
        public UnityTask(IEnumerator c, bool autoStart = true)
        {
            returnedValues = new Dictionary<string, object>();
            InitUnityTask(c, autoStart);
        }

        public UnityTask()
        {
            returnedValues = new Dictionary<string, object>();
        }

        public void CreateUnityTask(IEnumerator c, bool autoStart = true)
        {
            InitUnityTask(c, autoStart);
        }

        public static UnityTask CreateUnityTask<T1>(Func<UnityTask, T1, IEnumerator> coroutine, T1 arg1, bool autoStart = true)
        {
            var unityTask = new UnityTask();
            unityTask.InitUnityTask(coroutine(unityTask, arg1), autoStart);
            return unityTask;
        }

        public static UnityTask CreateUnityTask<T1, T2>(Func<UnityTask, T1, T2, IEnumerator> coroutine, T1 arg1, T2 arg2, bool autoStart = true)
        {
            var unityTask = new UnityTask();
            unityTask.InitUnityTask(coroutine(unityTask, arg1, arg2), autoStart);
            return unityTask;
        }

        public static UnityTask CreateUnityTask<T1, T2, T3>(Func<UnityTask, T1, T2, T3, IEnumerator> coroutine, T1 arg1, T2 arg2, T3 arg3, bool autoStart = true)
        {
            var unityTask = new UnityTask();
            unityTask.InitUnityTask(coroutine(unityTask, arg1, arg2, arg3), autoStart);
            return unityTask;
        }

        private void InitUnityTask(IEnumerator c, bool autoStart)
        {
            task = UnityTaskManager.CreateTask(c);
            task.Paused += TaskPaused;
            task.Resumed += TaskResumed;
            task.Finished += TaskFinished;
            if (autoStart)
                Start();
        }

        /// Returns true if and only if the coroutine is running.  Paused tasks
        /// are considered to be running.
        public bool Running
        {
            get { return task.Running; }
        }

        /// Returns true if and only if the coroutine is currently paused.
        public bool IsPaused
        {
            get { return task.IsPaused; }
        }

        /// Termination event.  Triggered when the coroutine completes execution.
        public event PausedHandler Paused;
        public event ResumedHandler Resumed;
        public event FinishedHandler Finished;

        public Dictionary<string, object> returnedValues;

        /// Begins execution of the coroutine
        public void Start()
        {
            task.Start();
        }

        /// Discontinues execution of the coroutine at its next yield.
        public void Stop()
        {
            task.Stop();
        }

        public void Pause()
        {
            task.Pause();
        }

        public void Unpause()
        {
            task.Unpause();
        }

        private void TaskFinished(bool manual)
        {
            Finished?.Invoke(manual, this);
        }

        private void TaskPaused()
        {
            Paused?.Invoke(this);
        }

        private void TaskResumed()
        {
            Resumed?.Invoke(this);
        }
    }

    internal class UnityTaskManager : MonoBehaviour
    {
        private static UnityTaskManager singleton;

        public static UnityTaskState CreateTask(IEnumerator coroutine)
        {
            if (singleton == null)
            {
                var go = new GameObject("TaskManager");
                DontDestroyOnLoad(go);
                singleton = go.AddComponent<UnityTaskManager>();
            }
            return new UnityTaskState(coroutine);
        }

        public class UnityTaskState
        {
            public delegate void PausedHandler();
            public delegate void ResumedHandler();
            public delegate void FinishedHandler(bool manual);

            private readonly IEnumerator coroutine;
            private bool stopped;

            public UnityTaskState(IEnumerator c)
            {
                coroutine = c;
            }

            public bool Running { get; private set; }

            public bool IsPaused { get; private set; }

            public event PausedHandler Paused;
            public event ResumedHandler Resumed;
            public event FinishedHandler Finished;

            public void Pause()
            {
                Paused?.Invoke();
                IsPaused = true;
            }

            public void Unpause()
            {
                Resumed?.Invoke();
                IsPaused = false;
            }

            public void Start()
            {
                Running = true;
                singleton.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                stopped = true;
                Running = false;
            }

            private IEnumerator CallWrapper()
            {
                //yield return null;
                var e = coroutine;
                while (Running)
                {
                    if (IsPaused)
                        yield return null;
                    else
                    {
                        if (e != null && e.MoveNext())
                            yield return e.Current;
                        else
                            Running = false;
                    }
                }
                Finished?.Invoke(stopped);
            }
        }
    }
}