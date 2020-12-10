using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TUNING;
using UnityEngine;

namespace OniPowerSensorMod
{

    public class OniPowerSensorConfig : IBuildingConfig
    {
        public static string ID = "OniPowerSensor";
        private static readonly string kanim = "OniPowerSensor_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            string id = OniPowerSensorConfig.ID;
            string kanim = OniPowerSensorConfig.kanim;
            float[] tieR0_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues none = TUNING.NOISE_POLLUTION.NONE;
            EffectorValues tieR0_2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, kanim, 30, 30f, tieR0_1, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR0_2, noise, 0.2f);
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.LOGICWATTAGESENSOR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.LOGICWATTAGESENSOR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LOGICWATTAGESENSOR.LOGIC_PORT_INACTIVE, false, false)
    };
            SoundEventVolumeCache.instance.AddVolume(OniPowerSensorConfig.kanim, "PowerSwitch_on", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume(OniPowerSensorConfig.kanim, "PowerSwitch_off", TUNING.NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, OniPowerSensorConfig.ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            OniPowerSensor logicWattageSensor = go.AddOrGet<OniPowerSensor>();
            logicWattageSensor.manuallyControlled = false;
            logicWattageSensor.activateOnHigherThan = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
        }
    }

}
