﻿namespace Core.Systems.DataPersistenceSystem
{
    public class GameData
    {
        public float SoundsVolume;
        public float MusicVolume;
        public byte Level;
        public ushort Score;

        public GameData()
        {
            SoundsVolume = 0.5f;
            MusicVolume = 0.5f;
            Level = 0;
            Score = 0;
        }
    }
}