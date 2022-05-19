using UnityEngine;

namespace Gameplay.Conrollers
{
    public class ArcadeModel : ISaveData 
    {
        public int Version { get; set; }
        
    }

    public interface ArcadeSaveInterface
    {
        
    }
    
    public class ArcadeSaveController : SaveDataBaseController<ArcadeModel>, ArcadeSaveInterface
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

        public override void InitializeSaveData(ArcadeModel data)
        {
        }

        public override void OnSaveDataLoaded()
        {
        }
    }
}