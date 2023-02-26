using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace YuoTools
{
    public class YuoValue
    {
        [ReadOnly] [ShowInInspector] public float Value { get; private set; }

        private float baseValue;

        public YuoValue(float value)
        {
            baseValue = value;
            UpdateValue();
        }

        /// <summary>
        ///  基础值
        /// </summary>
        [HorizontalGroup()]
        [ShowInInspector]
        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                UpdateValue();
            }
        }

        [ShowInInspector]
        [HorizontalGroup()]
        [ReadOnly]
        public float AdditionalValues { get; private set; }

        private Dictionary<int, float> addValue = new();

        public void AddAddValue(int id, float value)
        {
            if (addValue.ContainsKey(id))
            {
                addValue[id] = value;
            }
            else
            {
                addValue.Add(id, value);
            }

            UpdateValue();
        }

        public void RemoveAddValue(int id)
        {
            if (addValue.ContainsKey(id))
            {
                addValue.Remove(id);
            }

            UpdateValue();
        }

        /// <summary>
        ///  额外值
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public float AddValue
        {
            get
            {
                float value = 0;
                foreach (var v in addValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        private Dictionary<int, float> mulAddValue = new();

        public void AddMulAddValue(int id, float value)
        {
            if (mulAddValue.ContainsKey(id))
            {
                mulAddValue[id] = value;
            }
            else
            {
                mulAddValue.Add(id, value);
            }

            UpdateValue();
        }

        public void RemoveMulAddValue(int id)
        {
            if (mulAddValue.ContainsKey(id))
            {
                mulAddValue.Remove(id);
            }

            UpdateValue();
        }

        /// <summary>
        ///  额外值乘数
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public float MulAddValue
        {
            get
            {
                float value = 1;
                foreach (var v in mulAddValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        private Dictionary<int, float> mulBaseValue = new();

        public void AddMulBaseValue(int id, float value)
        {
            if (mulBaseValue.ContainsKey(id))
            {
                mulBaseValue[id] = value;
            }
            else
            {
                mulBaseValue.Add(id, value);
            }

            UpdateValue();
        }

        public void RemoveMulBaseValue(int id)
        {
            if (mulBaseValue.ContainsKey(id))
            {
                mulBaseValue.Remove(id);
            }

            UpdateValue();
        }

        /// <summary>
        ///  基础值乘数
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public float MulBaseValue
        {
            get
            {
                float value = 1;
                foreach (var v in mulBaseValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        private Dictionary<int, float> mulValue = new();

        public void AddMulValue(int id, float value)
        {
            if (mulValue.ContainsKey(id))
            {
                mulValue[id] = value;
            }
            else
            {
                mulValue.Add(id, value);
            }

            UpdateValue();
        }

        public void RemoveMulValue(int id)
        {
            if (mulValue.ContainsKey(id))
            {
                mulValue.Remove(id);
            }

            UpdateValue();
        }

        /// <summary>
        ///  最终值乘数
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public float MulValue
        {
            get
            {
                float value = 1;
                foreach (var v in mulValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        private Dictionary<int, FloatAction<YuoValue>> valueChange = new();

        public void AddValueChangeAction(int id, FloatAction<YuoValue> action)
        {
            if (valueChange.ContainsKey(id))
            {
                valueChange[id] = action;
            }
            else
            {
                valueChange.Add(id, action);
            }

            UpdateValue();
        }

        public void RemoveValueChangeAction(int id)
        {
            if (valueChange.ContainsKey(id))
            {
                valueChange.Remove(id);
            }
        }

        public void UpdateValue()
        {
            Value = (baseValue * MulBaseValue + AddValue * MulAddValue) * MulValue;
            foreach (var action in valueChange.Values)
            {
                Value += action(this);
            }

            AdditionalValues = Value - baseValue;
        }

        #region Operator

        public static float operator +(YuoValue a, YuoValue b)
        {
            return a.Value + b.Value;
        }

        public static float operator -(YuoValue a, YuoValue b)
        {
            return a.Value - b.Value;
        }

        public static float operator *(YuoValue a, YuoValue b)
        {
            return a.Value * b.Value;
        }

        public static float operator /(YuoValue a, YuoValue b)
        {
            return a.Value / b.Value;
        }

        public static float operator +(YuoValue a, float b)
        {
            return a.Value + b;
        }

        public static float operator -(YuoValue a, float b)
        {
            return a.Value - b;
        }

        public static float operator *(YuoValue a, float b)
        {
            return a.Value * b;
        }

        public static float operator /(YuoValue a, float b)
        {
            return a.Value / b;
        }

        public static float operator +(float a, YuoValue b)
        {
            return a + b.Value;
        }

        public static float operator -(float a, YuoValue b)
        {
            return a - b.Value;
        }

        public static float operator *(float a, YuoValue b)
        {
            return a * b.Value;
        }

        public static float operator /(float a, YuoValue b)
        {
            return a / b.Value;
        }

        public static bool operator >(YuoValue a, YuoValue b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(YuoValue a, YuoValue b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(YuoValue a, YuoValue b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(YuoValue a, YuoValue b)
        {
            return a.Value <= b.Value;
        }

        public static bool operator >(YuoValue a, float b)
        {
            return a.Value > b;
        }

        public static bool operator <(YuoValue a, float b)
        {
            return a.Value < b;
        }

        public static bool operator >=(YuoValue a, float b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(YuoValue a, float b)
        {
            return a.Value <= b;
        }

        public static bool operator >(float a, YuoValue b)
        {
            return a > b.Value;
        }

        public static bool operator <(float a, YuoValue b)
        {
            return a < b.Value;
        }

        public static bool operator >=(float a, YuoValue b)
        {
            return a >= b.Value;
        }

        public static bool operator <=(float a, YuoValue b)
        {
            return a <= b.Value;
        }

        #endregion
    }
}