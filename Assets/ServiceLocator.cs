using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ServiceLocator : SingletonBehaviour<ServiceLocator>
{
    [SerializeField] Service[] m_services;

    Dictionary<Type, Service> services = new Dictionary<Type, Service>();

    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_services.Length; i++)
        {
            if (services.TryAdd(m_services[i].GetType(), m_services[i]))
                m_services[i].Init();
        }
    }

    public T Get<T>() where T : Service
    {
        return (T)services[typeof(T)];
    }
}
