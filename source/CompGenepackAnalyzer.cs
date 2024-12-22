﻿using RimWorld;
using ReimaginedFoundation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReimaginedGenetics
{
    [StaticConstructorOnStartup]
    public class CompGenepackAnalyzer : ThingCompVirtualThingHolder
    {
        private bool _autoAnalyze;
        private bool _isAnalyzing;
        private Genepack _insertedGenepack;

        private int _analyzingWorkCost => 2000;
        private int _analyzingWorkLeft;

        //private static readonly CachedTexture ResearchGenepacksIcon = new CachedTexture("UI/Gizmos/RecombineGenes");
        private static readonly CachedTexture AnalyzeGenepacksIcon = new CachedTexture("UI/Gizmos/AutoLoadGenepack");
        private static readonly Texture2D CancelLoadingIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        public override bool CanReceiveThings => !_isAnalyzing;

        protected override void OnThingHauledInternal(ThingCount ingredient)
        {
            base.OnThingHauledInternal(ingredient);
            InsertGenepack(ingredient);
        }

        protected override void OnAllThingsHauled()
        {
            base.OnAllThingsHauled();
            StartAnalyzing();
        }

        // TOGO: No need to check it with each item inserted, move check into StartAnalyzing()
        private void InsertGenepack(ThingCount ingredient) 
        {
            Genepack genepack = ingredient.Thing as Genepack;
            if (genepack != null)
            {
                if (_insertedGenepack == null)
                {
                    _insertedGenepack = genepack;
                }
            }
        }

        // TODO: Needs complete rework
        public override string CompInspectStringExtra()
        {
            base.CompInspectStringExtra();

            string info = null;
            if (_isAnalyzing)
            {
                info = $"Analyzing: {_insertedGenepack.GeneSet.GenesListForReading.Select((GeneDef x) => x.LabelCap.ToString()).ToCommaList()} \n";
                info += $"Work left: {_analyzingWorkLeft / 20}";
            }
            else if (_expectedThings != null && _expectedThings.Count > 0)
            {
                info += "Waiting for: " +
                _expectedThings
                    .Select(thingPair => _expectedThings.Count > 1
                        ? $"{thingPair.Key.label} x{thingPair.Value}"
                        : thingPair.Key.label)
                    .ToCommaList();
            }
            else
            {
                info = "Idle";
            }

            return info;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (_isAnalyzing && _analyzingWorkLeft > 0) 
            {
                if (--_analyzingWorkLeft <= 0)
                {
                    CompleteAnalyzing();
                }
            }
        }

        private void CompleteAnalyzing() 
        {
            _insertedGenepack = null;
            _insertedThings.Clear();
            _isAnalyzing = false;
            if (_autoAnalyze) 
            {
                RequestThings();
            }
        }

        private void StartAnalyzing() 
        {
            _expectedThings.Clear();
            _analyzingWorkLeft = _analyzingWorkCost;
            _isAnalyzing = true;
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Log.Message("Initialize");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Log.Message($"PostSpawnSetup {respawningAfterLoad}");
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            Log.Message($"PostDeSpawn {map.ToString()}");
        }

        public override ThingCount GetRequestedThing()
        {
            foreach (var pair in _expectedThings)
            {
                Thing thing = FindThing(pair.Key);
                if (thing != null) 
                {
                    if (!_insertedThings.ContainsKey(pair.Key))
                    {
                        return new ThingCount(thing, pair.Value);
                    }
                    else if (_insertedThings[pair.Key].Count < pair.Value)
                    {
                        int requiredCount = pair.Value - _insertedThings[pair.Key].Count;
                        return new ThingCount(thing, requiredCount);
                    }
                }
            }

            return null;
        }

        public override List<ThingCount> GetRequestedThings()
        {
            List<ThingCount> requestedThings = new List<ThingCount>();

            foreach (var pair in _expectedThings)
            {
                Thing thing = FindThing(pair.Key);
                if (thing != null)
                {
                    if (!_insertedThings.ContainsKey(pair.Key))
                    {
                        requestedThings.Add(new ThingCount(thing, pair.Value));
                    }
                    else if (_insertedThings[pair.Key].Count < pair.Value)
                    {
                        int requiredCount = pair.Value - _insertedThings[pair.Key].Count;
                        requestedThings.Add(new ThingCount(thing, requiredCount));
                    }
                }
            }

            return requestedThings;
        }

        private Thing FindThing(ThingDef thingDef) 
        {
            Thing thing = parent.Map.listerThings.AllThings
                .FirstOrDefault(x =>
                    x.def.defName == thingDef.defName &&
                    !x.IsForbidden(Faction.OfPlayer));
            return thing;
        }

        protected void RequestThings() 
        {
            _expectedThings = new Dictionary<ThingDef, int>(2)
            {
                { ThingDefOf.Genepack, 1 },
                { InternalDefOf.Neutroamine, 10 }
            };

            _insertedThings = new Dictionary<ThingDef, ThingCount>(2);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return AutoGenepackAnalyzingCommand();

            if (_isAnalyzing ||
                (_expectedThings.Count> 0 && !_autoAnalyze)) 
            {
                yield return CancelCommand();
            }

            if (!Prefs.DevMode)
            {
                yield break;
            }
        }

        private Gizmo AutoGenepackAnalyzingCommand() 
        {
            return new Command_Toggle
            {
                defaultLabel = "Allow auto analyzing genepacks".Translate() + "...",
                defaultDesc = "AnalyzeGenepackDesc".Translate(),
                icon = AnalyzeGenepacksIcon.Texture,
                isActive = () => _autoAnalyze,
                toggleAction = delegate
                {
                    _autoAnalyze = !_autoAnalyze;
                    RequestThings();
                }
            };
        }

        private Gizmo CancelCommand()
        {
            return new Command_Action
            {
                defaultLabel = "Cancel".Translate() + "...",
                defaultDesc = "Cancel".Translate(),
                icon = CancelLoadingIcon,
                action = delegate
                {
                    StopAnalyzing();
                }
            };
        }

        private void StopAnalyzing() 
        {
            foreach (var itemCountPair in _insertedThings)
            {
                Thing thingInstance = GenSpawn.Spawn(itemCountPair.Value.Thing, parent.Position, parent.Map, default(Rot4));
                thingInstance.stackCount = itemCountPair.Value.Count;
            }

            _insertedGenepack = null;
            _expectedThings.Clear();
            _insertedThings.Clear();

            _isAnalyzing = false;

            if (_autoAnalyze) 
            {
                RequestThings();
            }
        }
    }
}
