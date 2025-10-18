using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.Notifications;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using LabFusion;
using LabFusion.Network;

namespace BDSN
{
    public static class BuildInfo
    {
        public const string Name = "BDSN";
        public const string Description = "A silly bone lab mod that uses webhooks to notify discord server when a bonelab server is up";
        public const string Author = "SillyAlex";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    // Configuration class
    [Serializable]
    public class BDSNConfig
    {
        public string webhookUrl = "";
        public bool enableNotifications = true;
        public string serverName = "Bonelab Server";
        public bool sendOnServerStart = true;
        public bool sendOnServerStop = true;
        public string webhookUsername = "BDSN Bot";
        public string webhookAvatarUrl = "https://support.discord.com/hc/user_images/PRywUXcqg0v5DD6s7C3LyQ.jpeg";
    }

    public class BDSN : MelonMod
    {
        public static Page MainPage;
        private static BDSNConfig config;
        private static string configPath;

        public override void OnInitializeMelon()
        {
            // Set up config path
            configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "BDSN_Config.json");

            // Load or create config
            LoadConfig();

            // Set up menu
            SetupMenu();

            // Hook into level loading
            Hooking.OnLevelLoaded += OnLevelLoaded;

            LoggerInstance.Msg("BDSN initialized!");
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string configJson = File.ReadAllText(configPath);
                    config = JsonConvert.DeserializeObject<BDSNConfig>(configJson);
                    LoggerInstance.Msg("Config loaded successfully!");
                }
                else
                {
                    // Create default config
                    config = new BDSNConfig();
                    SaveConfig();
                    LoggerInstance.Msg("Default config created. Please edit BDSN_Config.json in your UserData folder!");
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error loading config: {ex.Message}");
                config = new BDSNConfig(); // Use default config if loading fails
            }
        }

        private void SaveConfig()
        {
            try
            {
                string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, configJson);
                LoggerInstance.Msg("Config saved successfully!");
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error saving config: {ex.Message}");
            }
        }

        private void SetupMenu()
        {
            MainPage = Page.Root.CreatePage("<color=#7289DA>B</color><color=#6678B5>D</color><color=#5A6791>S</color><color=#4E566D>N</color>", Color.white, 0, true);

            // Add config status info
            MainPage.CreateFunction("Config Status", Color.cyan, () => {
                string status = string.IsNullOrEmpty(config.webhookUrl) ?
                    "Webhook not configured" :
                    "Webhook configured";

                var notification = new Notification
                {
                    Title = "BDSN Config Status",
                    Message = $"{status}\nConfig file: {Path.GetFileName(configPath)}",
                    Type = string.IsNullOrEmpty(config.webhookUrl) ? NotificationType.Warning : NotificationType.Information,
                    PopupLength = 4f,
                    ShowTitleOnPopup = true
                };
                Notifier.Send(notification);
            });

            // Add test message button
            MainPage.CreateFunction("Send Test Message", Color.green, () => {
                if (string.IsNullOrEmpty(config.webhookUrl))
                {
                    var notification = new Notification
                    {
                        Title = "BDSN Error",
                        Message = "Please configure your webhook URL first!",
                        Type = NotificationType.Error,
                        PopupLength = 3f,
                        ShowTitleOnPopup = true
                    };
                    Notifier.Send(notification);
                    return;
                }

                SendDiscordMessage("Test message from BDSN mod!");
            });

            // Add reload config button
            MainPage.CreateFunction("Reload Config", Color.yellow, () => {
                LoadConfig();
                var notification = new Notification
                {
                    Title = "BDSN",
                    Message = "Configuration reloaded!",
                    Type = NotificationType.Information,
                    PopupLength = 2f,
                    ShowTitleOnPopup = true
                };
                Notifier.Send(notification);
            });

            // Add open config folder button
            MainPage.CreateFunction("Open Config Folder", Color.white, () => {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", MelonEnvironment.UserDataDirectory);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error opening config folder: {ex.Message}");
                }
            });
        }

        static void SendDiscordMessage(string message)
        {
            if (string.IsNullOrEmpty(config.webhookUrl))
            {
                MelonLogger.Error("Webhook URL is not configured!");
                return;
            }

            if (!config.enableNotifications)
            {
                MelonLogger.Msg("Notifications are disabled in config.");
                return;
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/json");

                    // Create a more detailed payload
                    var payload = new
                    {
                        content = message,
                        username = config.webhookUsername,
                        avatar_url = config.webhookAvatarUrl,
                        embeds = new[]
                        {
                            new
                            {
                                title = config.serverName,
                                description = message,
                                color = 7506394, // Discord blurple color
                                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                footer = new
                                {
                                    text = "BDSN Mod v" + BuildInfo.Version
                                },
                                thumbnail = new
                                {
                                    url = config.webhookAvatarUrl
                                }
                            }
                        }
                    };

                    string jsonPayload = JsonConvert.SerializeObject(payload);
                    byte[] data = Encoding.UTF8.GetBytes(jsonPayload);

                    client.UploadData(config.webhookUrl, data);
                    MelonLogger.Msg("Discord message sent successfully!");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error sending Discord message: {ex.Message}");

                var notification = new Notification
                {
                    Title = "BDSN Error",
                    Message = "Failed to send Discord message. Check console for details.",
                    Type = NotificationType.Error,
                    PopupLength = 4f,
                    ShowTitleOnPopup = true
                };
                Notifier.Send(notification);
            }
        }

        private void OnLevelLoaded(LevelInfo info)
        {
            var notification = new Notification
            {
                Title = "BDSN | Ready",
                Message = config.enableNotifications ?
                    "BDSN is ready and notifications are enabled!" :
                    "BDSN is ready but notifications are disabled.",
                Type = NotificationType.Success,
                PopupLength = 3f,
                ShowTitleOnPopup = true
            };
            Notifier.Send(notification);

            // Send Discord notification if enabled and configured
            if (config.sendOnServerStart && config.enableNotifications && !string.IsNullOrEmpty(config.webhookUrl))
            {
                if (!ValidateServerConnection()) return;

                try 
                {
                    SendDiscordMessage("Server is up!");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error sending server start message: {ex.Message}");
                }
            }
        }

        public override void OnApplicationQuit()
        {
            // Send server stop notification if enabled
            if (config.sendOnServerStop && config.enableNotifications && !string.IsNullOrEmpty(config.webhookUrl))
            {
                if (!ValidateServerConnection()) return;

                try
                {
                    SendDiscordMessage($"{config.serverName} is going offline.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Error sending server stop message: {ex.Message}");
                }
            }
        }

        private bool ValidateServerConnection()
        {
            if (!NetworkInfo.HasServer)
            {
                MelonLogger.Warning("Not connected to Fusion server");
                return false;
            }

            if (!NetworkInfo.IsHost)
            {
                MelonLogger.Warning("User is not the server host");
                return false;
            }

            return true;
        }
    }
}