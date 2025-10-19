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

namespace BoneNotifier
{
    public static class BuildInfo
    {
        public const string Name = "Bone Notifier";
        public const string Description = "My second silly bone lab mod that uses webhooks to notify discord server when a bonelab server is up";
        public const string Author = "SillyAlex";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    // Configuration class
    [Serializable]
    public class BoneNotifierConfig
    {
        public string webhookUrl = "";
        public bool enableNotifications = true;
        public string serverName = "Bonelab Server";
        public bool sendOnServerStart = true;
        public bool sendOnServerStop = true;
        public string webhookUsername = "Bone Notifier";
        public string webhookAvatarUrl = "";
    }

    public class BoneNotifier : MelonMod
    {
        public static Page MainPage;
        private static BoneNotifierConfig config;
        private static string configPath;
        private static bool InServer = false;
        private bool lastServerState = false;

        public override void OnInitializeMelon()
        {
            // Set up config path
            configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "BoneNotifier_Config.json");

            // Load or create config
            LoadConfig();

            // Set up menu
            SetupMenu();

            // Hook into level loading
            Hooking.OnLevelLoaded += OnLevelLoaded;

            LoggerInstance.Msg("Bone Notifier initialized!");
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string configJson = File.ReadAllText(configPath);
                    config = JsonConvert.DeserializeObject<BoneNotifierConfig>(configJson);
                    LoggerInstance.Msg("Config loaded successfully!");
                }
                else
                {
                    // Create default config
                    config = new BoneNotifierConfig();
                    SaveConfig();
                    LoggerInstance.Msg("Default config created. Please edit BoneNotifier_Config.json in your UserData folder!");
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error loading config: {ex.Message}");
                config = new BoneNotifierConfig(); // Use default config if loading fails
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
            MainPage = Page.Root.CreatePage("<color=#7289DA>B</color><color=#6678B5>L</color><color=#5A6791>S</color><color=#4E566D>N</color>", Color.white, 0, true);

            // Add config status info
            MainPage.CreateFunction("Config Status", Color.cyan, () => {
                string status = string.IsNullOrEmpty(config.webhookUrl) ?
                    "Webhook not configured" :
                    "Webhook configured";

                var notification = new Notification
                {
                    Title = "BoneNotifier Config Status",
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
                        Title = "BoneNotifier Error",
                        Message = "Please configure your webhook URL first!",
                        Type = NotificationType.Error,
                        PopupLength = 3f,
                        ShowTitleOnPopup = true
                    };
                    Notifier.Send(notification);
                    return;
                }

                SendDiscordMessage("Test message from BoneNotifier!");
            });

            // Add reload config button
            MainPage.CreateFunction("Reload Config", Color.yellow, () => {
                LoadConfig();
                var notification = new Notification
                {
                    Title = "BoneNotifier",
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
                                    text = "BoneNotifier Mod v" + BuildInfo.Version
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

                    client.UploadDataAsync(new Uri(config.webhookUrl), data);
                    MelonLogger.Msg("Discord message sent successfully!");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error sending Discord message: {ex.Message}");

                var notification = new Notification
                {
                    Title = "BoneNotifier Error",
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
                Title = "BoneNotifier | Ready",
                Message = config.enableNotifications ?
                    "BoneNotifier is ready and notifications are enabled!" :
                    "BoneNotifier is ready but notifications are disabled.",
                Type = NotificationType.Success,
                PopupLength = 3f,
                ShowTitleOnPopup = true
            };
            Notifier.Send(notification);
        }

        public static void HostBYEBYENOT() 
        {
            try
            {
                if (config.sendOnServerStop && config.enableNotifications && !string.IsNullOrEmpty(config.webhookUrl))
                {
                    SendDiscordMessage("Server is down!");
                }

            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error sending server stop message: {e.Message}");
            }
        }

        public static void HostHELLONOT() 
        {
            if (config.sendOnServerStart && config.enableNotifications && !string.IsNullOrEmpty(config.webhookUrl) && InServer == true)
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

        public override void OnUpdate() // Runs once per frame.
        {
            bool currentServerState = NetworkInfo.HasServer;

            // Detect disconnect (was true, now false)
            if (lastServerState && !currentServerState)
            {
                InServer = false;
                HostBYEBYENOT();
            }

            // Detect connect (was false, now true)
            if (!lastServerState && currentServerState)
            {
                InServer = true;
                HostHELLONOT();
            }

            lastServerState = currentServerState;
        }

        private static bool ValidateServerConnection()
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