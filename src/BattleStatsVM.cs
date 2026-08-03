using TaleWorlds.Library;

namespace SkillXpAnnouncer
{
    public class BattleStatsVM : ViewModel
    {
        private string _text = string.Empty;
        private float _fontSize = 14f;
        private float _positionX = 20f;
        private float _positionY = 60f;

        [DataSourceProperty]
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        [DataSourceProperty]
        public float FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged("FontSize");
                }
            }
        }

        [DataSourceProperty]
        public float PositionX
        {
            get { return _positionX; }
            set
            {
                if (_positionX != value)
                {
                    _positionX = value;
                    OnPropertyChanged("PositionX");
                }
            }
        }

        [DataSourceProperty]
        public float PositionY
        {
            get { return _positionY; }
            set
            {
                if (_positionY != value)
                {
                    _positionY = value;
                    OnPropertyChanged("PositionY");
                }
            }
        }

        public void Refresh(MCMSettings cfg)
        {
            if (cfg != null)
            {
                FontSize = cfg.BattleStatsFontSize;
                PositionX = cfg.BattleStatsX;
                PositionY = cfg.BattleStatsY;
            }
            Text = HarmonyPatches.BuildBattleStatsText();
        }
    }
}
