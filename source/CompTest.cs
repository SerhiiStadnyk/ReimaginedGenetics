using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ReimaginedGenetics
{
    [StaticConstructorOnStartup]
    public class CompTest : ThingComp
    {
        private int _progress = -1;
        private ThingOwner _innerContainer;

        private static readonly CachedTexture RecombineIcon = new CachedTexture("UI/Gizmos/RecombineGenes");

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

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (_progress == -1f)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Recombine".Translate() + "...",
                    defaultDesc = "RecombineDesc".Translate(),
                    icon = RecombineIcon.Texture,
                    action = delegate
                    {
                        Log.Message("Action 1");
                        Find.WindowStack.Add(new Window_TestWindow(this));
                    }
                };
            }

            if (!Prefs.DevMode)
            {
                yield break;
            }
            yield return new Command_Action
            {
                defaultLabel = "DEBUG: Action 1",
                action = delegate
                {
                    Log.Message("Dev Action 1");
                }
            };
        }
    }
}
