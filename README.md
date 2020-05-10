# Rust User Management (C#)

## Description
A plugin created in C# for Rust (Oxide/UMod). This plugin interacts with the GFL API and assigns users their appropriate group on player connect or when the `um.reloadusers` console command is executed. This was initially released on GFL's GitLab [here](https://gitlab.gflclan.com).

## Installation.
Place the `UserManagement.cs` file inside of `RustServer/oxide/plugins`. Afterwards, edit `RustServer/oxide/config/UserManagement.json` to your needs.

Please also make sure you have existing groups made. You can create groups using `oxide.group add <group> <"[Title]"><rank>`.

## Default Config
```
{
  "Debug": false,
  "Enabled": true,
  "Endpoint": "donators",
  "RemoveExisting": true,
  "RemoveExistingNoGroup": true,
  "Token": "MY_AUTH_TOKEN",
  "URL": "https://api.domain.com/"
}
```

## Config Descriptions
* `Debug` => Whether or not to enable debugging within the plugin.
* `Enabled`=> Whether or not to enable the plugin.
* `Endpoint` => The API endpoint.
* `RemoveExisting` => If true, when given a group, it will remove any others that are a part of the plugin from the user.
* `RemoveExistingNoGroup` => If the user has no group, it will remove any groups that are a part of the plugin from the user.
* `Token` => The API's token that's set using the `Authorization` header.
* `URL` => The API URL (https/SSL is supported).

## Credits
* [Christian Deacon](https://www.linkedin.com/in/christian-deacon-902042186/) - Created plugin.