using System.Collections.Generic;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using static Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.Static;
using static Mutagen.Bethesda.FormKeys.SkyrimSE.HarvestThoseMammothSkulls.Flora;

namespace MakeFirewoodPilesIntoContainers
{
    class Program
    {
        private static readonly Dictionary<IFormLinkGetter<ISkyrimMajorRecordGetter>, IFormLink<IFloraGetter>>
            SkullReplacements = new()
            {
                {BoneMammothSkull1, cm_BoneMammothSkull1},
                {BoneMammothSkull2, cm_BoneMammothSkull2},
                {BoneMammothSkull3, cm_BoneMammothSkull3},
                {BoneMammothSkull4, cm_BoneMammothSkull4}
            };
        
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .SetTypicalOpen(GameRelease.SkyrimSE, "Harvest Those Mammoth Skulls_patch.esp")
                .AddRunnabilityCheck(state => state.LoadOrder.AssertHasMod(HarvestThoseMammothSkulls.ModKey))
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args);
        }

        private static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var placed in state.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(state.LinkCache))
            {
                if (SkullReplacements.TryGetValue(placed.Record.Base, out var foundSkull))
                {
                    placed.GetOrAddAsOverride(state.PatchMod)
                        .Base.SetTo(foundSkull);
                }
            }
        }
    }
}