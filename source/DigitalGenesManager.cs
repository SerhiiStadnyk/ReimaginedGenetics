using System.Collections.Generic;
using System.Text;
using Verse;

namespace ReimaginedGenetics
{
    public class DigitalGenesManager : GameComponent
    {
        public Dictionary<GeneDef, DigitalGene> _digitalGenes = new Dictionary<GeneDef, DigitalGene>();
        public IReadOnlyDictionary<GeneDef, DigitalGene> DigitalGenes => _digitalGenes;

        public List<DigitalGene> PendingResearches;

        public DigitalGenesManager(Game game)
        {
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            CacheGenes();

            StringBuilder logBuilder = new StringBuilder();
            foreach (var digitGenePair in _digitalGenes)
            {
                logBuilder.AppendLine($"Digit gene: {digitGenePair.Value.Gene.defName} {digitGenePair.Value.Progress} {digitGenePair.Value.Done}");
            }
            Log.Warning(logBuilder.ToString());

/*            GeneDef geneGrayHair = GeneUtility.GenesInOrder.Find((geneDef) => geneDef.defName == "Hair_Gray");
            _digitalizedGenes[geneGrayHair].AddProgress(69);*/
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _digitalGenes, "DigitalizedGenes", LookMode.Def, LookMode.Deep);

            StringBuilder logBuilder = new StringBuilder();
            logBuilder.AppendLine($"Saved genes count: {_digitalGenes.Count}");
            foreach (var digitGenePair in _digitalGenes)
            {
                logBuilder.AppendLine($"Digit gene: {digitGenePair.Value.Gene.defName} {digitGenePair.Value.Progress}/{digitGenePair.Value.ResearchProgressCost} {digitGenePair.Value.Done}");
            }
            Log.Warning(logBuilder.ToString());
        }

        List<GeneDef> _genes = new List<GeneDef>();
        private void CacheGenes()
        {
            _genes.AddRange(DefDatabase<GeneDef>.AllDefs);

            if (_digitalGenes == null) 
            {
                _digitalGenes = new Dictionary<GeneDef, DigitalGene>();
            }

            foreach (var gene in DefDatabase<GeneDef>.AllDefs) 
            {
                if (!_digitalGenes.ContainsKey(gene)) 
                {
                    _digitalGenes.Add(gene, new DigitalGene(gene, 0, false));
                }
            }
        }
    }
}
