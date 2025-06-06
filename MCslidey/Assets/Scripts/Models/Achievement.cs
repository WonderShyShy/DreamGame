using System.Threading.Tasks;

namespace Models
{
    public static class Achievement
    {
        private static AchievementRemoveNumber _removeNumberConfig = null;
        private static AchievementIceNumber _iceNumberConfig = null;
        private static AchievementDW _dieToLifeConfig = null;
        private static AchievementComboNumber _comboNumberConfig = null;
        private static AchievementSkillMoves _skillMovesConfig = null;
        private static AchievementStepScore _stepScoreConfig = null;

        public static async Task<bool> LoadData()
        {
            if (_removeNumberConfig == null)
            {
                _removeNumberConfig =  
                    JsonParse<AchievementRemoveNumber>.GetDataFromPath("data_achievement_removeNumber");
            }
            
            if (_iceNumberConfig == null)
            {
                _iceNumberConfig =  
                    JsonParse<AchievementIceNumber>.GetDataFromPath("data_achievement_iceNumber");
            }
            
            if (_dieToLifeConfig == null)
            {
                _dieToLifeConfig =  JsonParse<AchievementDW>.GetDataFromPath("data_achievement_dw");
            }
            
            if (_comboNumberConfig == null)
            {
                _comboNumberConfig =  
                    JsonParse<AchievementComboNumber>.GetDataFromPath("data_achievement_comboNumber");
            }
            
            if (_stepScoreConfig == null)
            {
                _stepScoreConfig =  
                    JsonParse<AchievementStepScore>.GetDataFromPath("data_achievement_stepScore");
            }
            
            if (_skillMovesConfig == null)
            {
                _skillMovesConfig =  
                    JsonParse<AchievementSkillMoves>.GetDataFromPath("data_achievement_skillMoves");
            }

            return true;
        }
        
        public static int GetPercentByRemoveNumber(int removeNumber)
        {
            for (var i = _removeNumberConfig.data.Length - 1; i >= 0; --i)
            {
                if (removeNumber >= _removeNumberConfig.data[i].remove_num)
                {
                    return _removeNumberConfig.data[i].percent;
                }
            }

            return 0;
        }

        public static int GetPercentByIceNumber(int iceNumber)
        {
            for (var i = _iceNumberConfig.data.Length - 1; i >= 0; --i)
            {
                if (iceNumber >= _iceNumberConfig.data[i].ice_num)
                {
                    return _iceNumberConfig.data[i].percent;
                }
            }

            return 0;
        }

        public static int GetPercentByDW(int dwCount)
        {
            for (var i = _dieToLifeConfig.data.Length - 1; i >= 0; --i)
            {
                if (dwCount >= _dieToLifeConfig.data[i].dw_num)
                {
                    return _dieToLifeConfig.data[i].percent;
                }
            }
            
            return 0;
        }

        public static int GetPercentByComboNumber(int comboNumber)
        {
            for (var i = _comboNumberConfig.data.Length - 1; i >= 0; --i)
            {
                if (comboNumber >= _comboNumberConfig.data[i].combo_num)
                {
                    return _comboNumberConfig.data[i].percent;
                }
            }
            
            return 0;
        }

        public static int GetPercentByStepScore(int stepScore)
        {
            for (var i = _stepScoreConfig.data.Length - 1; i >= 0; --i)
            {
                if (stepScore >= _stepScoreConfig.data[i].step_score_num)
                {
                    return _stepScoreConfig.data[i].percent;
                }
            }
            
            return 0;
        }

        public static int GetPercentBySkillMovesNumber(int skillMovesNumber)
        {
            for (var i = _skillMovesConfig.data.Length - 1; i >= 0; --i)
            {
                if (skillMovesNumber >= _skillMovesConfig.data[i].skill_num)
                {
                    return _skillMovesConfig.data[i].percent;
                }
            }
            
            return 0;
        }
    }
}
