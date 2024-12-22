using ReimaginedFoundation;
using Verse;
using Verse.AI;

namespace ReimaginedGenetics
{
    public class WorkGiver_HaulToDigitalGeneLab : WorkGiver_InsertIngredientsToCellBase<CompGenepackAnalyzer>
    {
        protected override ThingDef TargetThingDef => InternalDefOf.DigitalGeneLab;
        protected override JobDef TargetJobDef => InternalDefOf.HaulToDigitalGeneLab;
        protected override Job FallbackJob => null;
    }
}