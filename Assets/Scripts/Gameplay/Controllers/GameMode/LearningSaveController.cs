using Gameplay.MetroDisplay.Model;
using Gameplay.Statistics;

namespace Gameplay.Conrollers
{
    
    public class LearningModel : ISaveData 
    {
        public int Version { get; set; }
        public int tokens;
        public BoolRecord<MetroStation, int> unlockedStations = new BoolRecord<MetroStation, int>();
    }

    public interface LearningSaveInterface
    {
        public int tokens { get; set; }
        public BoolRecord<MetroStation, int> unlockedStations { get; set; }
    }

    
    public class LearningSaveController : SaveDataBaseController<LearningModel>, ArcadeSaveInterface, LearningSaveInterface
    {
        private string filename;
        public override int Version => 1;
        public override string Filename => filename;
        
        public override void SetFilename(string filename)
        {
            this.filename = filename;
        }

        public override void OnVersionChanged(int oldVersion)
        {
        }

        public override void InitializeSaveData(LearningModel data)
        {
        }

        public override void OnSaveDataLoaded()
        {
        }

        public int tokens
        {
            get => current.tokens;
            set => current.tokens = value;
        }

        public BoolRecord<MetroStation, int> unlockedStations
        {
            get => current.unlockedStations;
            set => current.unlockedStations = value;
        }
    }
}