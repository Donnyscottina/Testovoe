using System.Collections.Generic;
using ClickerTest.Clicker.Model;
using ClickerTest.Config;
using NUnit.Framework;
using UniRx;
using UnityEngine;

namespace ClickerTest.Tests
{
    public sealed class ClickerModelTests
    {
        private ClickerConfig _config;
        private CurrencyModel _currency;
        private EnergyModel _energy;
        private ClickerModel _clicker;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<ClickerConfig>();
            _currency = new CurrencyModel(_config);
            _energy = new EnergyModel(_config);
            _clicker = new ClickerModel(_config, _currency, _energy);
        }

        [TearDown]
        public void TearDown()
        {
            _clicker.Dispose();
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void Collect_SpendsEnergyAndAddsCurrency()
        {
            var collected = new List<CollectResult>();
            _clicker.Collected.Subscribe(collected.Add);

            Assert.IsTrue(_clicker.TryCollect(CollectSource.Manual));
            Assert.IsTrue(_clicker.TryCollect(CollectSource.Auto));

            Assert.AreEqual(_config.ClickReward + _config.AutoCollectReward, _currency.Balance.Value);
            Assert.AreEqual(_config.MaxEnergy - _config.ClickEnergyCost - _config.AutoCollectEnergyCost, _energy.Current.Value);
            Assert.AreEqual(2, collected.Count);
        }

        [Test]
        public void Collect_IsRejectedWithoutEnergy()
        {
            while (_energy.Current.Value > 0)
                _clicker.TryCollect(CollectSource.Manual);

            var balance = _currency.Balance.Value;
            var rejected = new List<CollectSource>();
            _clicker.Rejected.Subscribe(rejected.Add);

            Assert.IsFalse(_clicker.TryCollect(CollectSource.Auto));
            Assert.AreEqual(balance, _currency.Balance.Value);
            CollectionAssert.AreEqual(new[] { CollectSource.Auto }, rejected);
        }

        [Test]
        public void Energy_RestoreIsClampedByMax()
        {
            _energy.TrySpend(5);
            _energy.Restore(_config.EnergyRegenAmount);

            Assert.AreEqual(_config.MaxEnergy, _energy.Current.Value);
        }
    }
}
