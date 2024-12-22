using Verse;

namespace ReimaginedGenetics
{
    public class CompProperties_GenepackAnalyzer : CompProperties
    {
        public float researchSpeed = 1f;

        public CompProperties_GenepackAnalyzer()
        {
            compClass = typeof(CompGenepackAnalyzer);
        }
    }
}
