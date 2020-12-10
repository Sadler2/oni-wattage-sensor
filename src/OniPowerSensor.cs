using KSerialization;
using STRINGS;
using UnityEngine;

namespace OniPowerSensorMod
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class OniPowerSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
    {
        private static readonly EventSystem.IntraObjectHandler<OniPowerSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OniPowerSensor>((component, data) => component.OnCopySettings(data));
        [Serialize]
        public bool dirty = true;
        private readonly float maxWattage = 1.5f * Wire.GetMaxWattageAsFloat(Wire.WattageRating.Max50000);
        [Serialize]
        public float thresholdWattage;
        [Serialize]
        public bool activateOnHigherThan;
        private readonly float minWattage;
        private float currentWattage;
        private bool wasOn;
        [MyCmpAdd]
        private CopyBuildingSettings copyBuildingSettings;
        private int lastAnimState = -1;
        private float averageWattage = 0;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(326264352, OnCopySettingsDelegate);
        }

        private void OnCopySettings(object data)
        {
            OniPowerSensor component = ((GameObject)data).GetComponent<OniPowerSensor>();
            if (!(component != null))
                return;
            Threshold = component.Threshold;
            ActivateAboveThreshold = component.ActivateAboveThreshold;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            OnToggle += new System.Action<bool>(OnSwitchToggled);
            UpdateVisualState(true);
            UpdateLogicCircuit();
            wasOn = switchedOn;
        }

        public void Sim200ms(float dt)
        {
            currentWattage = Game.Instance.circuitManager.GetWattsUsedByCircuit(Game.Instance.circuitManager.GetCircuitID(Grid.PosToCell(this)));
            currentWattage = Mathf.Max(0.0f, currentWattage);
            averageWattage = averageWattage * 0.9f + currentWattage * 0.1f;

            UpdateVisualState(true);

            if (activateOnHigherThan)
            {
                if ((currentWattage <= (double)thresholdWattage || IsSwitchedOn) && (currentWattage > (double)thresholdWattage || !IsSwitchedOn))
                    return;
                Toggle();
            }
            else
            {
                if ((currentWattage < (double)thresholdWattage || !IsSwitchedOn) && (currentWattage >= (double)thresholdWattage || IsSwitchedOn))
                    return;
                Toggle();
            }
        }

        public float GetWattageUsed()
        {
            return currentWattage;
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            UpdateVisualState(false);
            UpdateLogicCircuit();
        }

        private void UpdateLogicCircuit()
        {
            GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
        }

        private void UpdateVisualState(bool force = false)
        {
            KBatchedAnimController component = GetComponent<KBatchedAnimController>();

            int animState;

            if (averageWattage < 1.0)
            {
                animState = 0;
            } else
            if (averageWattage < thresholdWattage*0.333)
            {
                animState = 1;
            }
            else
            if (averageWattage < thresholdWattage * 0.666)
            {
                animState = 2;
            }
            else
            if (averageWattage < thresholdWattage)
            {
                animState = 3;
            }
            else
            {
                animState = 4;
            }

            if (animState != lastAnimState)
            {
                component.Play(animState.ToString(), KAnim.PlayMode.Once, 1f, 0.0f);
                lastAnimState = animState;
            }
        }

        protected override void UpdateSwitchStatus()
        {
            GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive, null);
        }

        public float Threshold
        {
            get
            {
                return thresholdWattage;
            }
            set
            {
                thresholdWattage = value;
                dirty = true;
            }
        }

        public bool ActivateAboveThreshold
        {
            get
            {
                return activateOnHigherThan;
            }
            set
            {
                activateOnHigherThan = value;
                dirty = true;
            }
        }

        public float CurrentValue
        {
            get
            {
                return GetWattageUsed();
            }
        }

        public float RangeMin
        {
            get
            {
                return minWattage;
            }
        }

        public float RangeMax
        {
            get
            {
                return maxWattage;
            }
        }

        public float GetRangeMinInputField()
        {
            return minWattage;
        }

        public float GetRangeMaxInputField()
        {
            return maxWattage;
        }

        public LocString Title
        {
            get
            {
                return UI.UISIDESCREENS.WATTAGESWITCHSIDESCREEN.TITLE;
            }
        }

        public LocString ThresholdValueName
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE;
            }
        }

        public string AboveToolTip
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE_TOOLTIP_ABOVE;
            }
        }

        public string BelowToolTip
        {
            get
            {
                return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE_TOOLTIP_BELOW;
            }
        }

        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedWattage(value, GameUtil.WattageFormatterUnit.Watts, units);
        }

        public float ProcessedSliderValue(float input)
        {
            return Mathf.Round(input);
        }

        public float ProcessedInputValue(float input)
        {
            return input;
        }

        public LocString ThresholdValueUnits()
        {
            return UI.UNITSUFFIXES.ELECTRICAL.WATT;
        }

        public ThresholdScreenLayoutType LayoutType
        {
            get
            {
                return ThresholdScreenLayoutType.SliderBar;
            }
        }

        public int IncrementScale
        {
            get
            {
                return 1;
            }
        }

        public NonLinearSlider.Range[] GetRanges
        {
            get
            {
                return new NonLinearSlider.Range[4]
                {
                    new NonLinearSlider.Range(5f, 5f),
                    new NonLinearSlider.Range(35f, 1000f),
                    new NonLinearSlider.Range(50f, 3000f),
                    new NonLinearSlider.Range(10f, maxWattage)
                };
            }
        }
    }
}