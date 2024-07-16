public static class PlayerDataMappingExtension
{
    public static PlayerData GetPlayerData(this DataManager manager, int playerId)
    {
        var _loadPlayerData = manager.LoadPlayerData;
        int index = playerId + 1;
        if(_loadPlayerData.Count == 0 || _loadPlayerData.ContainsKey(index) == false)
        {
            return null;
        }

        return _loadPlayerData[index];
    }
}
