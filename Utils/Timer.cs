using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private class TimerCallback
    {
        public Action Callback; // Método a ejecutar
        public float TriggerTime; // Tiempo en segundos cuando se ejecutará
    }

    private class RecurringCallback
    {
        public Action Callback; // Método a ejecutar
        public float Interval; // Intervalo en que el callback se ejecutará
        public float NextTriggerTime; // Tiempo siguiente para ejecutar el callback
    }

    private List<TimerCallback> _callbacks = new List<TimerCallback>();
    private List<RecurringCallback> _recurringCallbacks = new List<RecurringCallback>();
    private float _currentTime = 0f; // Tiempo actual del cronómetro

    // Agregar un callback que se ejecutará una sola vez después de X segundos
    public void AddCallback(Action callback, float seconds)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));
        _callbacks.Add(new TimerCallback
        {
            Callback = callback,
            TriggerTime = _currentTime + seconds // Momento en que se ejecutará el callback
        });
    }

    // Agregar un callback que se ejecutará repetidamente cada X segundos
    public void AddRecurringCallback(Action callback, float interval)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));
        if (interval <= 0) throw new ArgumentOutOfRangeException(nameof(interval), "El intervalo debe ser mayor que 0.");

        _recurringCallbacks.Add(new RecurringCallback
        {
            Callback = callback,
            Interval = interval,
            NextTriggerTime = _currentTime + interval // Momento en que se ejecutará por primera vez
        });
    }

    // Limpiar todos los callbacks (tanto únicos como recurrentes)
    public void Clear()
    {
        _callbacks.Clear();
        _recurringCallbacks.Clear();
        _currentTime = 0f;
    }

    // Reiniciar el cronómetro, pero manteniendo los callbacks
    public void Restart()
    {
        _currentTime = 0f;

        // Reiniciar los tiempos de los callbacks recurrentes
        foreach (var recurringCallback in _recurringCallbacks)
        {
            recurringCallback.NextTriggerTime = recurringCallback.Interval;
        }
    }

    public void Tick()
    {
        // Avanzar el tiempo del cronómetro basado en el tiempo real transcurrido
        _currentTime += Time.deltaTime;

        // Verificar y ejecutar callbacks únicos
        for (int i = _callbacks.Count - 1; i >= 0; i--)
        {
            if (_currentTime >= _callbacks[i].TriggerTime)
            {
                _callbacks[i].Callback(); // Ejecutar callback
                _callbacks.RemoveAt(i);  // Eliminar de la lista
            }
        }

        // Verificar y ejecutar callbacks recurrentes
        foreach (var recurringCallback in _recurringCallbacks)
        {
            if (_currentTime >= recurringCallback.NextTriggerTime)
            {
                recurringCallback.Callback(); // Ejecutar callback
                recurringCallback.NextTriggerTime += recurringCallback.Interval; // Programar para la próxima vez
            }
        }
    }
}