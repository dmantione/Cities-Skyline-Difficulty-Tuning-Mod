using System.Collections.Generic;
using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Globalization;
using UnityEngine;
using DifficultyTuningMod.DifficultyOptions;

namespace DifficultyTuningMod
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get { return DTMLang.Text("MOD_NAME"); }
        }

        public string Description
        {
            get { return DTMLang.Text("MOD_DESCRIPTION"); }
        }


        #region Options UI

        private bool freeze = true;
        private UIDropDown ddDifficulty;

        private float textScaleBig = 1.2f;
        private float textScaleMedium = 1.0f;
        private float textScaleSmall = 0.8f;

        private Dictionary<UISlider, IDifficultyParameter> sliders = new Dictionary<UISlider, IDifficultyParameter>();

        private void addLabel(UIScrollablePanel panel, string text, Vector3 position, float scale)
        {
            UILabel label = panel.AddUIComponent<UILabel>();
            label.text = text;
            label.textScale = scale;
            label.relativePosition = position;
        }

        private void addSlider(UIScrollablePanel panel, Vector3 position, float width, OnValueChanged eventCallback, IDifficultyParameter param)
        {
            UISlider slider = panel.AddUIComponent<UISlider>();
            slider.size = new Vector2(width, 8);
            slider.relativePosition = position;
            slider.minValue = 0;
            slider.maxValue = param.MaxIndex;
            slider.stepSize = 1;
            slider.value = param.SelectedIndex;
            slider.backgroundSprite = "TextFieldPanel"; // TextFieldPanel, GenericProgressBar

            UISprite thumb = slider.AddUIComponent<UISprite>();
            thumb.size = new Vector2(16, 16);
            thumb.spriteName = "InfoIconBaseFocused"; // SliderBudget, InfoIconBaseFocused
            slider.thumbObject = thumb;

            UILabel label = panel.AddUIComponent<UILabel>();
            label.textScale = textScaleSmall;
            label.text = param.GetValueStr((int)slider.value);
            label.relativePosition = new Vector3(position.x + width + 10, position.y);

            slider.eventValueChanged += delegate (UIComponent c, float val)
            {
                label.text = param.GetValueStr((int)val);
                if (!freeze)
                {
                    param.SelectedIndex = (int)slider.value;
                    eventCallback(val);
                }
            };

            sliders.Add(slider, param);
        }

        private string truncateSemicolon(string str)
        {
            return str.Split(':')[0];
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            DifficultyManager d = Singleton<DifficultyManager>.instance;

            ddDifficulty = (UIDropDown)helper.AddDropdown(
                DTMLang.Text("DIFFICULTY_LEVEL"),
                d.DifficultiesStr,
                (int)d.Difficulty,
                DifficultyLevelOnSelected
                );
            ddDifficulty.width = 315;
            ddDifficulty.height -= 2;

            //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, ddDifficulty.parent.parent.ToString());

            UIScrollablePanel scrollablePanel = (UIScrollablePanel)ddDifficulty.parent.parent;
            scrollablePanel.autoLayout = false;

            float x1 = 15;
            float x2 = x1 + 140;
            float x3 = x1 + 375;
            float x4 = x3 + 140;
            float y = 0;
			float dy1 = 24;
            float dy2 = 44;
            float w1 = 150;
            float w2 = w1 + 140;

            ddDifficulty.parent.relativePosition = new Vector3(5, y);

            UIButton btn = (UIButton)helper.AddButton(Locale.Get("SAVE"), saveBtnClicked);
            btn.autoSize = false;
            btn.textHorizontalAlignment = UIHorizontalAlignment.Center;
            btn.width = 200;
            btn.height = ddDifficulty.height;
            btn.relativePosition = new Vector3(x3, y + 24);

            y += ddDifficulty.parent.height + 10;


            //
            // Custom options
            //
            sliders.Clear();

            addLabel(scrollablePanel, DTMLang.Text("CUSTOM_OPTIONS"), new Vector3(5, y), textScaleBig);
            y += dy2;

            // Construction cost
            addLabel(scrollablePanel, truncateSemicolon(Locale.Get("TOOL_CONSTRUCTION_COST")), new Vector3(x2, y), textScaleMedium);
            addLabel(scrollablePanel, DTMLang.Text("SERVICE_BUILDINGS"), new Vector3(x1, y + dy1), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y + dy1), w1, OnCustomValueChanged, d.ConstructionCostMultiplier_Service);
            addLabel(scrollablePanel, DTMLang.Text("PUBLIC_TRANSPORT"), new Vector3(x1, y + dy1 * 2), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y + dy1 * 2), w1, OnCustomValueChanged, d.ConstructionCostMultiplier_Public);
            addLabel(scrollablePanel, DTMLang.Text("ROADS"), new Vector3(x1, y + dy1 * 3), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y + dy1 * 3), w1, OnCustomValueChanged, d.ConstructionCostMultiplier_Road);
            addLabel(scrollablePanel, DTMLang.Text("OTHERS"), new Vector3(x1, y + dy1 * 4), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y + dy1 * 4), w1, OnCustomValueChanged, d.ConstructionCostMultiplier);

            // Maintenance cost
            addLabel(scrollablePanel, truncateSemicolon(Locale.Get("AIINFO_UPKEEP")), new Vector3(x4, y), textScaleMedium);
            addLabel(scrollablePanel, DTMLang.Text("SERVICE_BUILDINGS"), new Vector3(x3, y + dy1), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x4, y + dy1), w1, OnCustomValueChanged, d.MaintenanceCostMultiplier_Service);
            addLabel(scrollablePanel, DTMLang.Text("PUBLIC_TRANSPORT"), new Vector3(x3, y + dy1 * 2), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x4, y + dy1 * 2), w1, OnCustomValueChanged, d.MaintenanceCostMultiplier_Public);
            addLabel(scrollablePanel, DTMLang.Text("ROADS"), new Vector3(x3, y + dy1 * 3), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x4, y + dy1 * 3), w1, OnCustomValueChanged, d.MaintenanceCostMultiplier_Road);
            addLabel(scrollablePanel, DTMLang.Text("OTHERS"), new Vector3(x3, y + dy1 * 4), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x4, y + dy1 * 4), w1, OnCustomValueChanged, d.MaintenanceCostMultiplier);
            y += dy1 * 4;
            y += dy2;

            // Relocate cost
            addLabel(scrollablePanel, truncateSemicolon(Locale.Get("TOOL_RELOCATE_COST")), new Vector3(x1, y), textScaleMedium);
            y += dy1;
            addSlider(scrollablePanel, new Vector3(x1, y), w2, OnCustomValueChanged, d.RelocationCostMultiplier);
            y -= dy1;

            // Area purchase cost
            addLabel(scrollablePanel, DTMLang.Text("AREA_COST_MULTIPLIER"), new Vector3(x3, y), textScaleMedium);
            y += dy1;
            addSlider(scrollablePanel, new Vector3(x3, y), w2, OnCustomValueChanged, d.AreaCostMultiplier);
            y += dy2;

            // Initial money
            addLabel(scrollablePanel, DTMLang.Text("INITIAL_MONEY"), new Vector3(x1, y), textScaleMedium);
            y += dy1;
            addSlider(scrollablePanel, new Vector3(x1, y), w2, OnCustomValueChanged, d.InitialMoney);
            y -= dy1;

            // Reward amount
            addLabel(scrollablePanel, DTMLang.Text("REWARD"), new Vector3(x3, y), textScaleMedium);
            y += dy1;
            addSlider(scrollablePanel, new Vector3(x3, y), w2, OnCustomValueChanged, d.RewardMultiplier);
            y += dy2;

            // Demand
            addLabel(scrollablePanel, DTMLang.Text("DEMAND"), new Vector3(x1, y), textScaleMedium);
            y += dy1;
            addLabel(scrollablePanel, DTMLang.Text("DEMAND_OFFSET"), new Vector3(x1, y), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y), w1, OnCustomValueChanged, d.DemandOffset);
            y += dy1;
            addLabel(scrollablePanel, DTMLang.Text("DEMAND_MULTIPLIER"), new Vector3(x1, y), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y), w1, OnCustomValueChanged, d.DemandMultiplier);
            y += dy2;

            // Target land value
            addLabel(scrollablePanel, DTMLang.Text("TAGRET_LANDVALUE"), new Vector3(x1, y), textScaleMedium);
            y += dy1;
            addLabel(scrollablePanel, DTMLang.Text("RESIDENTIAL"), new Vector3(x1, y), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y), w1, OnCustomValueChanged, d.ResidentialTargetLandValue);
            y += dy1;
            addLabel(scrollablePanel, DTMLang.Text("COMMERCIAL"), new Vector3(x1, y), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y), w1, OnCustomValueChanged, d.CommercialTargetLandValue);
            y += dy2;

            // Target service score
            addLabel(scrollablePanel, DTMLang.Text("TAGRET_SCORE"), new Vector3(x1, y), textScaleMedium);
            y += dy1;
            addLabel(scrollablePanel, DTMLang.Text("INDUSTRIAL"), new Vector3(x1, y), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y), w1, OnCustomValueChanged, d.IndustrialTargetScore);
            y += dy1;
            addLabel(scrollablePanel, DTMLang.Text("OFFICE"), new Vector3(x1, y), textScaleSmall);
            addSlider(scrollablePanel, new Vector3(x2, y), w1, OnCustomValueChanged, d.OfficeTargetScore);

            freeze = false;
        }

        private void DifficultyLevelOnSelected(int sel)
        {
            if (freeze) return;

            DifficultyManager d = Singleton<DifficultyManager>.instance;
            
            d.Difficulty = (Difficulties)sel;
            d.Modified = true;
            
            // Update controls
            freeze = true;
            foreach (KeyValuePair<UISlider, IDifficultyParameter> item in sliders)
            {
                item.Key.value = item.Value.SelectedIndex;
            }
            freeze = false;

            Achievements.Update();
        }

        private void OnCustomValueChanged(float val)
        {
            if (freeze) return;

            DifficultyManager d = Singleton<DifficultyManager>.instance;
            
            d.Difficulty = Difficulties.Custom;
            d.Modified = true;
            
            freeze = true;
            ddDifficulty.selectedIndex = (int)Difficulties.Custom;
            freeze = false;

            Achievements.Update();
        }

        private void saveBtnClicked()
        {
            DifficultyManager d = Singleton<DifficultyManager>.instance;

            if (d != null && d.Modified)
            {
                d.Save();
            }
        }

        #endregion
    }
}
