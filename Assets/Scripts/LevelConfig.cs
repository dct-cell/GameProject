using System;
using System.Collections.Generic;

[Serializable]
public class AnSingleEnemy {
    public string name;
    public int xPos;
    public int yPos;
    public int zPos;
}

[Serializable]
public class LevelConfig {
    public int maxCharacterCount;
    public AnSingleEnemy[] enemyList;
}