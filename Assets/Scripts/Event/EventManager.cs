using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GenericEvent<T> : UnityEvent<T> { }
[Serializable]
public class GenericEvent<T, K> : UnityEvent<T, K> { }
[Serializable]
public class GenericEvent<T, K, J> : UnityEvent<T, K, J> { }

public class EventManager<E> where E : Enum 
{
    private static readonly Dictionary<E, UnityEventBase> EventDic = new Dictionary<E, UnityEventBase>();

    private static readonly object LockObj = new object();

    #region Binding
    public static void Binding(bool isBind, E eventName, UnityAction method)
    {
        if (isBind) AddBinding(eventName, method);
        else RemoveBinding(eventName, method);
    }

    public static void Binding<T>(bool isBind, E eventName, UnityAction<T> method)
    {
        if (isBind) AddBinding(eventName, method);
        else RemoveBinding(eventName, method);
    }

    public static void Binding<T, K>(bool isBind, E eventName, UnityAction<T, K> method)
    {
        if (isBind) AddBinding(eventName, method);
        else RemoveBinding(eventName, method);
    }

    public static void Binding<T, K, J>(bool isBind, E eventName, UnityAction<T, K, J> method)
    {
        if (isBind) AddBinding(eventName, method);
        else RemoveBinding(eventName, method);
    }
    #endregion
    #region Add Binding
    private static void AddBinding(E eventName, UnityAction method)
    {
        lock(LockObj)
        {
            UnityEvent unityEvent = GetOrCreateEvent<UnityEvent>(eventName);
            unityEvent.AddListener(method);
        }
    }

    private static void AddBinding<T>(E eventName, UnityAction<T> method)
    {
        lock (LockObj)
        {
            GenericEvent<T> genericEvent = GetOrCreateEvent<GenericEvent<T>>(eventName);
            genericEvent.AddListener(method);
        }
    }

    private static void AddBinding<T, K>(E eventName, UnityAction<T, K> method)
    {
        lock (LockObj)
        {
            GenericEvent<T, K> genericEvent = GetOrCreateEvent<GenericEvent<T, K>>(eventName);
            genericEvent.AddListener(method);
        }
    }

    private static void AddBinding<T, K, J>(E eventName, UnityAction<T, K, J> method)
    {
        lock (LockObj)
        {
            GenericEvent<T, K, J> genericEvent = GetOrCreateEvent<GenericEvent<T, K, J>>(eventName);
            genericEvent.AddListener(method);
        }
    }
    #endregion
    #region RemoveBinding
    private static void RemoveBinding(E eventName, UnityAction method)
    {
        lock (LockObj)
        {
            if(EventDic.TryGetValue(eventName, out var thisEvent) 
                && thisEvent is UnityEvent unityEvent)
            {
                unityEvent.RemoveListener(method);
                RemoveEventIfEmpty(eventName, unityEvent);
            }
        }
    }

    private static void RemoveBinding<T>(E eventName, UnityAction<T> method)
    {
        lock (LockObj)
        {
            if (EventDic.TryGetValue(eventName, out var thisEvent) 
                && thisEvent is GenericEvent<T> genericEvent)
            {
                genericEvent.RemoveListener(method);
                RemoveEventIfEmpty(eventName, genericEvent);
            }
        }
    }

    private static void RemoveBinding<T, K>(E eventName, UnityAction<T, K> method)
    {
        lock (LockObj)
        {
            if (EventDic.TryGetValue(eventName, out var thisEvent)
                && thisEvent is GenericEvent<T, K> genericEvent)
            {
                genericEvent.RemoveListener(method);
                RemoveEventIfEmpty(eventName, genericEvent);
            }
        }
    }
    private static void RemoveBinding<T, K, J>(E eventName, UnityAction<T, K, J> method)
    {
        lock (LockObj)
        {
            if (EventDic.TryGetValue(eventName, out var thisEvent)
                && thisEvent is GenericEvent<T, K, J> genericEvent)
            {
                genericEvent.RemoveListener(method);
                RemoveEventIfEmpty(eventName, genericEvent);
            }
        }
    }
    #endregion
    #region Trigger
    public static void TriggerEvent(E eventName)
    {
        InvokeEvent(eventName);
    }
    public static void TriggerEvent<T>(E eventName, T parameter1)
    {
        InvokeEvent(eventName, parameter1);
    }
    public static void TriggerEvent<T, K>(E eventName, T parameter1, K parameter2)
    {
        InvokeEvent(eventName, parameter1, parameter2);
    }
    public static void TriggerEvent<T, K, J>(E eventName, T parameter1, K parameter2, J parameter3)
    {
        InvokeEvent(eventName, parameter1, parameter2, parameter3);
    }
    #endregion
    #region EventInvoke
    // 이벤트 실행
    private static void InvokeEvent(E eventName)
    {
        lock (LockObj)
        {
            try
            {
                if (EventDic.TryGetValue(eventName, out var thisEvent) &&
                thisEvent is UnityEvent unityEvent)
                {
                    unityEvent.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{eventName} 이벤트 오류 발생 : {ex.Message}");
            }
        }
    }
    private static void InvokeEvent<T>(E eventName, T parameter1)
    {
        lock (LockObj)
        {
            try
            {
                if (EventDic.TryGetValue(eventName, out var thisEvent) &&
                thisEvent is GenericEvent<T> unityEvent)
                {
                    unityEvent.Invoke(parameter1);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{eventName} 이벤트 오류 발생 : {ex.Message}");
            }
        }
    }
    private static void InvokeEvent<T, K>(E eventName, T parameter1, K parameter2)
    {
        lock (LockObj)
        {
            try
            {
                if (EventDic.TryGetValue(eventName, out var thisEvent) &&
                thisEvent is GenericEvent<T, K> unityEvent)
                {
                    unityEvent.Invoke(parameter1, parameter2);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{eventName} 이벤트 오류 발생 : {ex.Message}");
            }
        }
    }
    private static void InvokeEvent<T, K, J>(E eventName, T parameter1, K parameter2, J parameter3)
    {
        lock (LockObj)
        {
            try
            {
                if (EventDic.TryGetValue(eventName, out var thisEvent) &&
                thisEvent is GenericEvent<T, K, J> unityEvent)
                {
                    unityEvent.Invoke(parameter1, parameter2, parameter3);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{eventName} 이벤트 오류 발생 : {ex.Message}");
            }
        }
    }
    #endregion
    // 이벤트가 존재하지 않으면 생성하여 반환하는 메서드
    private static TEvent GetOrCreateEvent<TEvent>(E eventName) where TEvent : UnityEventBase, new()
    {
        if (!EventDic.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent = new TEvent();
            EventDic.Add(eventName, thisEvent);
        }

        return thisEvent as TEvent;
    }

    // 이벤트가 비어 있으면 딕셔너리에서 제거하는 메서드
    private static void RemoveEventIfEmpty(E eventName, UnityEventBase thisEvent)
    {
        if (thisEvent.GetPersistentEventCount() == 0)
        {
            EventDic.Remove(eventName);
        }
    }


}
