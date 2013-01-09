using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;


namespace ShadowMain
{
    class SaveIO
    {

        // Savedata structure - IN PROGRESS
        public struct SaveData
        {
            public string bgID;
            public string skinID;
            public string foreID;
        }

        //Storage
        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,
            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToSave
        }
        StorageDevice storageDevice;
        SavingState savingState = SavingState.NotSaving;
        IAsyncResult asyncResult;
        PlayerIndex playerIndex = PlayerIndex.One;
        StorageContainer storageContainer;
        string filename = "savegame.sav";
        SaveData saveGameData = new SaveData()
        {
            bgID = "x",
            skinID = "y",
            foreID = "z"
        };

        public void UpdateSaveKey()
        {
                if (savingState == SavingState.NotSaving)
                {
                    savingState = SavingState.ReadyToOpenStorageContainer;
                    Debug.WriteLine("SaveCompleted!");
                }
        }

        public void UpdateSaving()
        {
            switch (savingState)
            {
                case SavingState.ReadyToSelectStorageDevice:
                    {
                        asyncResult = StorageDevice.BeginShowSelector(playerIndex, null, null);
                        savingState = SavingState.SelectingStorageDevice;
                    }
                    break;

                case SavingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    break;

                case SavingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        savingState = SavingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Game1StorageContainer", null, null);
                        savingState = SavingState.OpeningStorageContainer;
                    }
                    break;

                case SavingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        savingState = SavingState.ReadyToSave;
                    }
                    break;

                case SavingState.ReadyToSave:
                    if (storageContainer == null)
                    {
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    else
                    {
                        try
                        {
                            DeleteExisting();
                            Save();
                        }
                        catch (IOException e)
                        {
                            // Replace with in game dialog notifying user of error
                            Debug.WriteLine(e.Message);
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            savingState = SavingState.NotSaving;
                        }
                    }
                    break;
            }
        }

        private void DeleteExisting()
        {
            if (storageContainer.FileExists(filename))
            {
                storageContainer.DeleteFile(filename);
            }
        }

        private void Save()
        {
            using (Stream stream = storageContainer.CreateFile(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(stream, saveGameData);
            }
        }

    }
}
