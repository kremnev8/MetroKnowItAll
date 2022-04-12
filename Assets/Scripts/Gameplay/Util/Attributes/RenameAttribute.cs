using UnityEngine;

namespace Util
{

    public class RenameAttribute : PropertyAttribute
    {
        public string NewName { get ; private set; }    
        public RenameAttribute( string name )
        {
            NewName = name ;
        }
    }
}