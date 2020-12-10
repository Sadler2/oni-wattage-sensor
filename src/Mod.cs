using Database;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OniPowerSensorMod
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class Mod
    {

        private static void Prefix()
        {
            if (Localization.GetLocale() != null && Localization.GetLocale().Lang == Localization.Language.Russian)
            {
                Strings.Add("STRINGS.BUILDINGS.PREFABS.ONIPOWERSENSOR.NAME", "Продвинутый сенсор мощности");
                Strings.Add("STRINGS.BUILDINGS.PREFABS.ONIPOWERSENSOR.EFFECT", "Сенсор мощности с отображением текущей мощности сети");
                Strings.Add("STRINGS.BUILDINGS.PREFABS.ONIPOWERSENSOR.DESC", "Сенсор мощности из партии с исправными индикаторами");
            }
            else
            {
                Strings.Add("STRINGS.BUILDINGS.PREFABS.ONIPOWERSENSOR.NAME", "Advanced Wattage Sensor");
                Strings.Add("STRINGS.BUILDINGS.PREFABS.ONIPOWERSENSOR.EFFECT", "Wattage sensor with display of the current network power");
                Strings.Add("STRINGS.BUILDINGS.PREFABS.ONIPOWERSENSOR.DESC", "Wattage sensor from a batch with properly working indicators");
            }

            ModUtil.AddBuildingToPlanScreen("Automation", OniPowerSensorConfig.ID);
            List<string> ls = new List<string>(Techs.TECH_GROUPING["AdvancedPowerRegulation"]) { OniPowerSensorConfig.ID };
            Techs.TECH_GROUPING["AdvancedPowerRegulation"] = ls.ToArray();
        }
        private static void Postfix()
        {
            
        }
    }
}
