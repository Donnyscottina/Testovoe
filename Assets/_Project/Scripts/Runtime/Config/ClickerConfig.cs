using UnityEngine;

namespace ClickerTest.Config
{
    [CreateAssetMenu(menuName = "Clicker Test/Clicker Config", fileName = "ClickerConfig")]
    public sealed class ClickerConfig : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField, Min(0)] private int _startCurrency;
        [SerializeField, Min(0)] private int _clickReward = 1;
        [SerializeField, Min(0)] private int _autoCollectReward = 1;

        [Header("Energy")]
        [SerializeField, Min(1)] private int _maxEnergy = 1000;
        [SerializeField, Min(0)] private int _startEnergy = 1000;
        [SerializeField, Min(0)] private int _clickEnergyCost = 1;
        [SerializeField, Min(0)] private int _autoCollectEnergyCost = 1;
        [SerializeField, Min(0)] private int _energyRegenAmount = 10;
        [SerializeField, Min(0.1f)] private float _energyRegenIntervalSeconds = 10f;

        [Header("Auto collect")]
        [SerializeField, Min(0.1f)] private float _autoCollectIntervalSeconds = 3f;

        public int StartCurrency => _startCurrency;
        public int ClickReward => _clickReward;
        public int AutoCollectReward => _autoCollectReward;

        public int MaxEnergy => _maxEnergy;
        public int StartEnergy => Mathf.Min(_startEnergy, _maxEnergy);
        public int ClickEnergyCost => _clickEnergyCost;
        public int AutoCollectEnergyCost => _autoCollectEnergyCost;
        public int EnergyRegenAmount => _energyRegenAmount;
        public float EnergyRegenIntervalSeconds => _energyRegenIntervalSeconds;

        public float AutoCollectIntervalSeconds => _autoCollectIntervalSeconds;
    }
}
