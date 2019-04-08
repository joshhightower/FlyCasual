using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Tokens;
using System.Linq;
using Editions;
using SubPhases;

namespace ActionsList
{

    public class ReloadAction : GenericAction
    {

        public ReloadAction()
        {
            Name = DiceModificationName = "Reload";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }

        public override void ActionTake()
        {
            Edition.Current.ReloadAction(this);
        }

        public static void FlipFaceupRecursive(GenericAction action)
        {
            GenericUpgrade discardedUpgrade = null;

            List<GenericUpgrade> discardedUpgrades = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyDiscarded();
            if (discardedUpgrades.Count != 0) discardedUpgrade = discardedUpgrades.FirstOrDefault(n => n.HasType(UpgradeType.Missile) || n.HasType(UpgradeType.Torpedo));

            if (discardedUpgrade != null)
            {
                discardedUpgrade.FlipFaceup(delegate { FlipFaceupRecursive(action); });
            }
            else
            {
                AssignTokenAndFinish(action);
            }
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int discardedOrdnance = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyDiscarded().Count(n => n.HasType(UpgradeType.Missile) || n.HasType(UpgradeType.Torpedo));
            result = discardedOrdnance * 30;

            return result;
        }

        private static List<GenericUpgrade> GetReloadableUpgrades()
        {
            return Selection.ThisShip.UpgradeBar.GetRechargableUpgrades(new List<UpgradeType> { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Bomb });
        }

        public static void RestoreOneCharge(GenericAction action)
        {
            List<GenericUpgrade> rechargableUpgrades = GetReloadableUpgrades();

            if (rechargableUpgrades.Count > 1)
            {
                StartDecisionSubphase(action);
            }
            else if (rechargableUpgrades.Count == 1)
            {
                rechargableUpgrades[0].State.RestoreCharge();
                Messages.ShowInfo(Selection.ThisShip.PilotInfo.PilotName + " recharges 1 charge of " + rechargableUpgrades[0].UpgradeInfo.Name + " and gains a Disarmed token.");
                AssignTokenAndFinish(action);
            }
            else
            {
                Messages.ShowError("This ship has no upgrades that can have their charges restored.");
                action.FinishAction();
            }
        }

        private static void StartDecisionSubphase(GenericAction action)
        {
            ReloadDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<ReloadDecisionSubphase>(
                "Choose one device to reload",
                delegate { AssignTokenAndFinish(action); }
            );

            subphase.InfoText = "Choose one device to regain one charge";
            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            foreach (GenericUpgrade upgrade in GetReloadableUpgrades())
            {
                subphase.AddDecision(upgrade.UpgradeInfo.Name, delegate { RechargeUpgrade(upgrade); }, upgrade.ImageUrl, upgrade.State.Charges);
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        protected static void AssignTokenAndFinish(GenericAction action)
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), action.FinishAction);
        }

        private static void RechargeUpgrade(GenericUpgrade upgrade)
        {
            upgrade.State.RestoreCharge();
            Messages.ShowInfo("Reload: One charge of \"" + upgrade.UpgradeInfo.Name + "\" is restored.");

            DecisionSubPhase.ConfirmDecision();
        }

        public class ReloadDecisionSubphase : DecisionSubPhase { }

    }

}
