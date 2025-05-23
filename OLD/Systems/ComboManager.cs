using System;
using DefaultNamespace;
using UnityEngine;

public class ComboManager
{
    private Combo[] _myCombos;
    private readonly float _comboCooldown;
    private int _currentAttackIndex = 0;
    private int _currentComboIndex = 0;
    private float _lastAttackTime = 0;

    public ComboManager(float comboCooldown, Combo[] combos)
    {
        if (comboCooldown == 0)
        {
            throw new Exception("Combo cooldown cannot be zero.");
        }

        _comboCooldown = comboCooldown;
        _myCombos = combos;
    }

    private bool CanContinueCombo(float attackTime)
    {
        bool canContinue = attackTime - _lastAttackTime < _comboCooldown;
        _lastAttackTime = attackTime;
        return canContinue;
    }

    public void GetAttackAndUpdateIndex(out Attack attack)
    {
        if (_myCombos == null)
        {
            Debug.LogError("ComboManager: _myCombos is null.");
            throw new Exception("No combos available in ComboManager.");
        }

        if (_myCombos.Length == 0)
        {
            Debug.LogError("ComboManager: _myCombos length is zero.");
            throw new Exception("No combos available in ComboManager.");
        }

        if (!CanContinueCombo(Time.time))
        {
            UpdateComboIndex();
            _currentAttackIndex = 0;
        }

        attack = _myCombos[_currentComboIndex].AttackSet[_currentAttackIndex];


        // At the beginning of the combo
        if (_currentAttackIndex == 0)
        {
            _currentComboIndex = UpdateComboIndex();
        }

        _currentAttackIndex = UpdateAttackIndex();
    }

    public bool IsCurrentAttackLastInCombo()
    {
        if (_myCombos == null || _myCombos.Length == 0)
        {
            throw new InvalidOperationException("No combos available in ComboManager.");
        }
        
        var currentCombo = _myCombos[_currentComboIndex];

        if (currentCombo.AttackSet == null || currentCombo.AttackSet.Length == 0)
        {
            throw new InvalidOperationException("Current combo has no attacks defined.");
        }

        return _currentAttackIndex == currentCombo.AttackSet.Length - 1;
    }
    private int UpdateAttackIndex()
    {
        _currentAttackIndex = (_currentAttackIndex + 1) % _myCombos[_currentComboIndex].AttackSet.Length;
        return _currentAttackIndex;
    }

    public int UpdateComboIndex()
    {
        _currentComboIndex = (_currentComboIndex + 1) % _myCombos.Length;
        _currentAttackIndex = 0;
        return _currentComboIndex;
    }

    public Combo GetRandomComboByIndex()
    {
        if (_myCombos == null || _myCombos.Length == 0)
        {
            throw new Exception("No combos available in ComboManager.");
        }

        var combo = _myCombos[UpdateComboIndex()];

        if (combo.AttackSet.Length == 0)
        {
            throw new Exception("Combo is empty");
        }

        return combo;
    }

    public Combo GetRandomCombo()
    {
        if (_myCombos == null || _myCombos.Length == 0)
        {
            throw new Exception("No combos available in ComboManager.");
        }

        var index = UnityEngine.Random.Range(0, _myCombos.Length);
        var combo = _myCombos[index];

        if (combo.AttackSet.Length == 0)
        {
            throw new Exception("Combo is empty");
        }

        return combo;
    }
    
    public Attack GetRandomAttack()
    {
        if (_myCombos == null || _myCombos.Length == 0)
        {
            throw new Exception("No combos available in ComboManager.");
        }

        // Seleccionar un combo aleatorio
        var randomComboIndex = UnityEngine.Random.Range(0, _myCombos.Length);
        var randomCombo = _myCombos[randomComboIndex];

        if (randomCombo.AttackSet == null || randomCombo.AttackSet.Length == 0)
        {
            throw new Exception("Selected combo has no attacks.");
        }

        // Seleccionar un ataque aleatorio dentro del combo
        var randomAttackIndex = UnityEngine.Random.Range(0, randomCombo.AttackSet.Length);
        return randomCombo.AttackSet[randomAttackIndex];
    }

}