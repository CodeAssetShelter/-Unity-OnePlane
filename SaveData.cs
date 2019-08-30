using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// 190510 LifeBalance
// SaveData Area
public class SaveData : MonoBehaviour
{
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SaveData)) as SaveData;

                if (_instance == null)
                {
                    Debug.LogError("No Active SaveData!");
                }
            }

            return _instance;
        }
    }

    // Save Data
    [Serializable]
    public class OnePlaneData
    {
        // Game Data
        public Planes.GoodsInfo[] goodsInfo;
        public int credit;
        public int bestScore;
        // Config Data
    }

    public Planes gamePlanes;
    private OnePlaneData saveData = new OnePlaneData();


    // 190510 LifeBalance
    // Save Area
    public void SaveUserData()
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/OnePlane.dat");

        OnePlaneData data = new OnePlaneData();

        //data.goodsInfo = new Planes.GoodsInfo[gamePlanes.GetGoodsEa()];
        //for (int i = 0; i < gamePlanes.GetGoodsEa(); i++)
        //{
        //    data.goodsInfo[i] = gamePlanes.goodsInfo[i];
        //}

        data.goodsInfo = gamePlanes.goodsInfo;

        data.credit = PublicValueStorage.Instance.RefreshCredit(0);
        data.bestScore = PublicValueStorage.Instance.GetBestScore();

        binary.Serialize(file, data);

        saveData = data;

        file.Close();
    }



    // 190510 LifeBalance
    // Load Area
    public void LoadUserData()
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/OnePlane.dat", FileMode.Open);


        if (file != null && file.Length > 0)
        {
            OnePlaneData data = (OnePlaneData)binary.Deserialize(file);

            this.saveData = data;
            gamePlanes.goodsInfo = this.saveData.goodsInfo;
        }
    }

    //public Planes LoadPlanesData()
    //{
    //    return saveData.goodsInfo;
    //}

    public int LoadBestScore()
    {
        return saveData.bestScore;
    }

    public int LoadCredit()
    {
        return saveData.credit;
    }
}
