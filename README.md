<h1 align="center">🦴 BDSN — Bonelab Discord Server Notifier</h1>

<p align="center">
  A lightweight and silly mod for <b>Bonelab</b> that sends webhook notifications to your Discord server when your Fusion server goes online or offline.
</p>

---

## 📜 Overview

**BDSN** is a mod designed for server owners who want to keep their communities informed.  
When your server starts (or stops), BDSN automatically sends a message to your configured Discord webhook.

✨ **Features**:
- ✅ Discord webhook integration  
- 🖥️ Automatic notifications on server start and stop  
- 🪄 Custom server name and avatar  
- 🧾 Easy configuration via JSON  
- 📢 In-game menu controls through **BoneMenu**

---

## 🧰 Requirements

-  "Lakatrazz-Fusion-1.12.2"
-  "gnonme-BoneLib-3.1.2"
-  Internet connection (for Discord webhook requests)

---

## 📥 Installation

Automatic
Use a Thunderstore Mod Manager (e.g. Thunderstore Mod Manager, r2modman, Gale, etc.) to install the mod. This should also install BDSN's dependencies alongside BDSN, too.

Manual
These instructions are for Steam installations, but Meta PC App installations shouldn't be too far off.

Install MelonLoader, BoneLib and Fusion if you haven't already.
Download this mod via the "Manual Download" button.
Extract the contents of the ZIP folder that was just downloaded
(named Popper-BDSN-X.X.X.zip)
Open your Steam Library, right-click on BONELAB, hover over Manage and click Browse Local Files. This should open another window/tab that shows BONELAB's installation.
Copy the BDSN.dll file from the extracted ZIP folder from step 3 and paste it into the Mods folder in BONELAB's installation.
Open the game via Steam, and you should be able to access the mod through the BoneMenu in-game!

---

## ⚙️ Configuration

Once launched, a file named `BDSN_Config.json` will be created in: Userdata folder with these options 


Inside, you’ll find these options:

```json
{
  "webhookUrl": "",
  "enableNotifications": false,
  "serverName": "test server",
  "sendOnServerStart": true,
  "sendOnServerStop": false,
  "webhookUsername": "BDSN Bot",
  "webhookAvatarUrl": ""
}
```
webhookUrl – Your Discord webhook URL.


enableNotifications – Enables or disables sending messages to Discord.

true = Send notifications

false = Do not send anything

serverName – The name that will appear in the Discord embed (e.g., “My Bonelab Server”).

sendOnServerStart – If true, BDSN will send a Discord message when the server starts.

sendOnServerStop – If true, BDSN will send a Discord message when the server stops.

webhookUsername – Name that appears as the webhook sender in Discord.

webhookAvatarUrl – URL to an image used as the bot avatar (recommended size: 256x256).