﻿namespace CodeBase.Core.Systems.DataPersistenceSystem
{
    public interface IDataPersistence
    {
        void LoadData(GameData gameData);
        void SaveData(ref GameData gameData);
    }
}