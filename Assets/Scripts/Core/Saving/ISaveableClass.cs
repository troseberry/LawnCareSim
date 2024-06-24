using System;
using System.Collections;

namespace Core.Saving
{
    public interface ISaveableClass
    {
        public SaveableSceneObjectType SaveableType { get; }

        public ISaveableClassData SaveData { get; }

        public void LoadClassData(ISaveableClassData toLoad);

        public IEnumerator LoadClassDataAsync(ISaveableClassData toLoad);

        public void SaveClassData();
    }
}
