using UnityEngine;
/// <summary>
/// Any class inheriting from this class will be auto-registered on the ServicesManager.
/// Use it for Audio Services, Haptic Services, etc.
/// </summary>
public class BaseService : MonoBehaviour
{
    protected virtual void Awake() => ServicesManager.Get().RegisterService(this);

}