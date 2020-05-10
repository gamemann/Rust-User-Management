using System;
using System.Collections.Generic;
using System.Linq;

using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Logging;

using Newtonsoft.Json.Linq;

namespace Oxide.Plugins
{
    [Info("GFL's User Management", "Roy (Christian Deacon)", "0.1")]
    
    public class UserManagement : RustPlugin
    {
        void OnPlayerConnected(BasePlayer player)
        {
            // Check if enabled.
            if ((bool)Config["Enabled"])
            {
                // Load the user.
                LoadUser(player);
            }
        }

        // Reload all users command.
        [ConsoleCommand("um.reloadusers")]
        private void ReloadusersCommand(ConsoleSystem.Arg arg)
        {
            if (!arg.IsAdmin || !(bool)Config["Enabled"])
            {
                arg.ReplyWith("You have no power here. >:D");

                return;
            }

            foreach (var i in BasePlayer.activePlayerList)
            {
                LoadUser(i);
            }

            arg.ReplyWith("Reloaded all users successfully.");
        }

        protected override void LoadDefaultConfig()
        {
            // Set defaults.
            Config["Enabled"] = true;
            Config["URL"] = "https://api.domain.com/";
            Config["Endpoint"] = "donators";
            Config["Token"] = "MY_AUTH_TOKEN";
            Config["Debug"] = false;
            Config["RemoveExisting"] = true;
            Config["RemoveExistingNoGroup"] = true;
        }

        // Loads a user.
        void LoadUser(BasePlayer player)
        {
            // Get the player's Steam ID 64.
            String steamid = player.UserIDString;
            String name = player.displayName;

            // Add a debug message because why not...
            if ((bool)Config["Debug"])
            {
                Puts($"[DEBUG] Loading {name} ({steamid})...");
            }

            // Compile the headers (for the token).
            Dictionary<string, string> headers = new Dictionary<string, string>();

            // Add the authorization token.
            headers.Add("Authorization", Config["Token"].ToString());

            // Send the HTTP GET request.
            webrequest.Enqueue(Config["URL"].ToString() + Config["Endpoint"].ToString() + "?steamid=" + steamid, null, (code, response) =>
            {
                // Check for errors.
                if (code != 200 || response == null)
                {
                    // Log error.
                    Puts($"Error with GET request for {name} ({steamid}). URL - " + Config["URL"].ToString() + ", Endpoint = " + Config["Endpoint"].ToString() + ", Token = " + Config["Token"].ToString() + ", Code = " + code);

                    return;
                }

                // Parse request as JSON.
                var json = JObject.Parse(response);

                int err = (int)json.GetValue("error");

                // Check for invalid token.
                if (err == 401)
                {
                    Puts($"Invalid token for GET request. {name} ({steamid}). URL - " + Config["URL"].ToString() + ", Endpoint = " + Config["Endpoint"].ToString() + ", Token = " + Config["Token"].ToString() + ", Code = " + code);

                    return;
                }

                int groupID = (int)json.GetValue("group");

                if ((bool)Config["Debug"])
                {
                    Puts($"Loaded user {name} ({steamid}) with group ID {groupID}");
                }

                // Add the user to whatever group they're supposed to be in and remove other groups.
                if (groupID == 1)
                {
                    player.IPlayer.AddToGroup("member");

                    // Remove existing groups if need to be.
                    if ((bool)Config["RemoveExisting"])
                    {
                        RemoveGroup(player, "supporter");
                        RemoveGroup(player, "vip");
                    }
                }
                else if (groupID == 2)
                {
                    player.IPlayer.AddToGroup("supporter");

                    // Remove existing groups if need to be.
                    if ((bool)Config["RemoveExisting"])
                    {
                        RemoveGroup(player, "member");
                        RemoveGroup(player, "vip");
                    }
                }
                else if (groupID == 3)
                {
                    player.IPlayer.AddToGroup("vip");

                    // Remove existing groups if need to be.
                    if ((bool)Config["RemoveExisting"])
                    {
                        RemoveGroup(player, "member");
                        RemoveGroup(player, "supporter");
                    }
                }
                else
                {
                    // I don't know if Rust "sticks" with groups, so if someone loses VIP, this option will ensure they're not in that group anymore.
                    if ((bool)Config["RemoveExistingNoGroup"])
                    {
                        // Member.
                        RemoveGroup(player, "member");

                        // Supporter.
                        RemoveGroup(player, "supporter");

                        // VIP.
                        RemoveGroup(player, "vip");
                    }
                }
            }, this, RequestMethod.GET, headers);
        }

        // Helper.
        void RemoveGroup (BasePlayer player, String group)
        {
            if (player.IPlayer.BelongsToGroup(group))
            {
                player.IPlayer.RemoveFromGroup(group);

                if ((bool)Config["Debug"])
                {
                    Puts($"Removed " + player.displayName + " (" + player.UserIDString + ") from " + group);
                }
            }
        }
    }
}