using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayerDataSaver
{
    public static void ErasePlayerDatas()
    {
        #region Player Equipments
        string equipmentsPath = Application.persistentDataPath + "/playerEquipments.data";
        if(File.Exists(equipmentsPath))
            File.Delete(equipmentsPath);
        #endregion

        #region Player Progression
        string progressionPath = Application.persistentDataPath + "/playerProgression.data";
        if (File.Exists(progressionPath))
            File.Delete(progressionPath);
        #endregion

        #region Shops Parameters
        #endregion
    }

    #region Equipments
    public static void SavePlayerEquipmentsDatas(EquipmentsSet equipmentSet, List<ShipEquipment> inventoryEquipments, int armorAmount, int goldAmount)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerEquipments.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerEquipmentsDatas data = new PlayerEquipmentsDatas();
        data.SetPlayerEquipmentsData(equipmentSet, inventoryEquipments, armorAmount, goldAmount);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerEquipmentsDatas(PlayerEquipmentsDatas data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerEquipments.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerEquipmentsDatas LoadPlayerEquipmentsDatas()
    {
        string path = Application.persistentDataPath + "/playerEquipments.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerEquipmentsDatas data = formatter.Deserialize(stream) as PlayerEquipmentsDatas;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("File does not exist");
            return null;
        }
    }
    #endregion

    #region Progression
    /*public static void SavePlayerProgressionDatas(List<PassedArenaData> passedArenaDatas)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerProgression.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerProgressionDatas data = new PlayerProgressionDatas();
        data.SetProgressionDatas(passedArenaDatas);

        formatter.Serialize(stream, data);
        stream.Close();
    }*/

    public static void SavePlayerProgressionDatas(List<PassedArenaData> passedArenaDatas, bool passedTutorial)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerProgression.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerProgressionDatas data = new PlayerProgressionDatas();
        data.SetProgressionDatas(passedArenaDatas);
        if(passedTutorial)
            data.SetPassedTutorial();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerProgressionDatas(PlayerProgressionDatas data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerProgression.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerProgressionDatas LoadProgressionDatas()
    {
        string path = Application.persistentDataPath + "/playerProgression.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerProgressionDatas data = formatter.Deserialize(stream) as PlayerProgressionDatas;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("File does not exist");
            return null;
        }
    }

    public static void PrintProgressionDatas()
    {
        /*PlayerProgressionDatas data = LoadProgressionDatas();
        data.debu;*/
    }
    #endregion

    #region Shops Parameters
    public static void SaveSellersDatas(List<ShopWithShopParameters> allShopsParameters)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/shopDatas.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        SellersData data = new SellersData();
        data.SetSellersData(allShopsParameters);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveSellersDatas(SellersData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/shopDatas.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SellersData LoadShopDatas()
    {
        string path = Application.persistentDataPath + "/shopDatas.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SellersData data = formatter.Deserialize(stream) as SellersData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("File does not exist");
            return null;
        }
    }

    #endregion 
}
