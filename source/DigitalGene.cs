using UnityEngine;
using Verse;

namespace ReimaginedGenetics
{
    public class DigitalGene : IExposable
    {
        private GeneDef _gene;
        private float _progress;
        private bool _done;

        public GeneDef Gene => _gene;
        public float Progress => _progress;
        public bool Done => _done;

        public float ResearchProgressCost => 100 + Mathf.Abs(_gene.biostatCpx * 100) + Mathf.Abs(_gene.biostatMet * 200) + _gene.biostatArc * 2000;

        public DigitalGene(GeneDef gene, float progress, bool done)
        {
            _gene = gene;
            _progress = progress;
            _done = done;

            if (_done) 
            {
                _progress = ResearchProgressCost;
            }
        }

        public DigitalGene()
        {
        }

        public void AddProgress(float value) 
        {
            _progress = Mathf.Min(_progress + value, ResearchProgressCost);
            if (!_done && _progress >= ResearchProgressCost) 
            {
                _done = true;
                // TODO: Invoke action on progress changed 
            }
            Log.Warning($"Progress is {_progress} after increasing it by {value}");
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref _gene, "geneDef");
            Scribe_Values.Look(ref _progress, "_progress");
            Scribe_Values.Look(ref _done, "_done");

            if (_done)
            {
                _progress = ResearchProgressCost;
            }
        }
    }
}
