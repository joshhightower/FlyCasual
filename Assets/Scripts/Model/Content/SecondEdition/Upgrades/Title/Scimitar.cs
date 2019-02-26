using Upgrade;
using BoardTools;
using System.Linq;
using Actions;
using ActionsList;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class Scimitar : GenericUpgrade
    {
        public Scimitar() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Scimitar",
                UpgradeType.Title,
                cost: 10,       
                isLimited: true,
                addActions: new List<ActionInfo>(){
                    new ActionInfo(typeof(CloakAction), ActionColor.Red, this),
                    new ActionInfo(typeof(JamAction), ActionColor.White, this),
                },
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.SecondEdition.SithInfiltrator.SithInfiltrator)),
                    new FactionRestriction(Faction.Separatists)
                ),
                abilityType: typeof(Abilities.SecondEdition.ScimitarAbility)
            );

            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/05/58/05586413-d67b-4313-b47d-20a5ef22b00b/swz30_scimitar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScimitarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}