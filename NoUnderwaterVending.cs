using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("No Underwater Vending", "VisEntities", "1.0.0")]
    [Description(" ")]
    public class NoUnderwaterVending : RustPlugin
    {
        #region Fields

        private static NoUnderwaterVending _plugin;

        #endregion Fields

        #region Oxide Hooks

        private void Init()
        {
            _plugin = this;
        }

        private void Unload()
        {
            _plugin = null;
        }

        private object CanBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            if (planner == null || prefab == null)
                return null;

            BasePlayer player = planner.GetOwnerPlayer();
            if (player == null)
                return null;

            Item activeItem = player.GetActiveItem();
            if (activeItem == null || !activeItem.info.shortname.Contains("vending.machine"))
                return null;

            if (target.entity == null || target.entity.ShortPrefabName != "wall.doorway")
                return null;

            if (WaterLevel.Test(target.position + Quaternion.Euler(target.rotation) * target.GetWorldPosition() - new Vector3(0f, 0.1f, 0f), true, true, null))
            {
                SendMessage(player, Lang.CannotPlaceUnderwater);
                return true;
            }

            return null;
        }

        #endregion Oxide Hooks

        #region Localization

        private class Lang
        {
            public const string CannotPlaceUnderwater = "CannotPlaceUnderwater";
        }

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [Lang.CannotPlaceUnderwater] = "You cannot place a vending machine underwater."
            }, this, "en");
        }

        private void SendMessage(BasePlayer player, string messageKey, params object[] args)
        {
            string message = lang.GetMessage(messageKey, this, player.UserIDString);
            if (args.Length > 0)
                message = string.Format(message, args);

            SendReply(player, message);
        }

        #endregion Localization
    }
}