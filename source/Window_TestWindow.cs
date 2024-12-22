using ReimaginedFoundation;
using RimWorld;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace ReimaginedGenetics
{
    [StaticConstructorOnStartup]
    public class Window_TestWindow : Window
    {
        protected static readonly Vector2 ButtonSize = new Vector2(150f, 38f);

        protected bool alwaysUseFullBiostatsTableHeight;
        protected bool ignoreRestrictions;
        protected bool xenotypeNameLocked;
        protected float postXenotypeHeight;
        protected float searchWidgetOffsetX;
        protected string xenotypeName;

        protected int archites;
        protected int geneComplexitry;
        protected int metabolism;
        protected int maxGeneComplexitry = -1;

        protected XenotypeIconDef iconDef;
        protected QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

        private static readonly Regex ValidSymbolRegex = new Regex("^[\\p{L}0-9 '\\-]*$");
        public static readonly Texture2D UnlockedTex = ContentFinder<Texture2D>.Get("UI/Overlays/LockedMonochrome");
        public static readonly Texture2D LockedTex = ContentFinder<Texture2D>.Get("UI/Overlays/Locked");

        protected static readonly float _genesWidgetsSpacing = 16f;

        public override Vector2 InitialSize => new Vector2(1016f, UI.screenHeight);
        protected string Header => "AssembleGenes".Translate();

        private List<GeneDef> tmpGenes = new List<GeneDef>();
        private List<Genepack> selectedGenepacks = new List<Genepack>();
        protected List<GeneDef> SelectedGenes
        {
            get
            {
                tmpGenes.Clear();
                foreach (Genepack selectedGenepack in selectedGenepacks)
                {
                    foreach (GeneDef item in selectedGenepack.GeneSet.GenesListForReading)
                    {
                        tmpGenes.Add(item);
                    }
                }
                return tmpGenes;
            }
        }






        private ScrollArea _genesLibraryScrollArea;
        private List<DigitalGeneWidget> _digitalGeneWidgets;


        public Window_TestWindow(CompTest compTest)
        {
            _genesLibraryScrollArea = new ScrollArea();

            DigitalGenesManager digitalGenesManager = Current.Game.GetComponent<DigitalGenesManager>();
            _digitalGeneWidgets = new List<DigitalGeneWidget>(digitalGenesManager.DigitalGenes.Count);
            foreach (var digitalGene in digitalGenesManager.DigitalGenes)
            {
                _digitalGeneWidgets.Add(new DigitalGeneWidget(digitalGene.Value));
            }
            Log.Warning($"Digital Genes Widgets: {_digitalGeneWidgets.Count}");
            Log.Warning($"Digital Gene Manager Genes: {digitalGenesManager._digitalGenes.Count}");
        }

        public override void DoWindowContents(Rect rect)
        {
            GenesLibrarryWindow(rect);
        }

        protected void UpdateSearchResults() 
        {
        }
/*        protected void DrawGenes(Rect rect) 
        {
            GUI.BeginGroup(rect);
            Rect rect2 = new Rect(0f, 0f, rect.width - 16f, scrollHeight);
            float curY = 0f;
            Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, rect2);
            Rect containingRect = rect2;
            containingRect.y = scrollPosition.y;
            containingRect.height = rect.height;
            DrawSection(rect, selectedGenepacks, "SelectedGenepacks".Translate(), ref curY, ref selectedHeight, adding: false, containingRect);
            curY += 8f;
            DrawSection(rect, libraryGenepacks, "GenepackLibrary".Translate(), ref curY, ref unselectedHeight, adding: true, containingRect);
            if (Event.current.type == EventType.Layout)
            {
                scrollHeight = curY;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
        }*/

        private float HeaderHeight = 35f;
        private float SearchAreaHeight = 24f;
        private float _geneLibraryHeight = 700f;
        public void GenesLibrarryWindow(Rect rect)
        {
            // Draw header
            Rect headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
            DrawHeader(headerRect);

            // Adjust main content area
            Rect contentRect = rect;
            contentRect.yMin += HeaderHeight + Margin;

            // Draw search field
            Rect searchRect = DrawSearchRect(contentRect);

            // Allocate space for gene table
            Rect genesRect = contentRect;
            genesRect.yMin = searchRect.yMax + Margin;
            genesRect.height = _geneLibraryHeight;
            DrawGenesLibrary(genesRect);

            // Allocate space for gene table
            Rect geneStatsRect = contentRect;
            geneStatsRect.yMin = genesRect.yMax + Margin;
            DrawGenesStats(geneStatsRect);

            // Draw bottom buttons
            Rect bottomButtonRect = new Rect(rect.x, rect.yMax - ButtonSize.y, rect.width, ButtonSize.y);
            DrawBottomButtons(bottomButtonRect);
        }

        private void DrawHeader(Rect rect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, Header);
            Text.Font = GameFont.Small;
        }

        protected Rect DrawSearchRect(Rect rect)
        {
            Rect searchRect = new Rect(rect.width - 300f - searchWidgetOffsetX, 11f, 300f, SearchAreaHeight);
            quickSearchWidget.OnGUI(searchRect, UpdateSearchResults);

            return searchRect;
        }

        protected void DrawGenesLibrary(Rect rect)
        {
            List<IDraweable> geneWidgets = new List<IDraweable>(_digitalGeneWidgets);
            _genesLibraryScrollArea.DrawGrid(rect, geneWidgets, _genesWidgetsSpacing, new Vector2(0.5f, 0));
        }

/*        private static readonly List<GeneDef> _geneDefs = new List<GeneDef>();
        private static readonly Dictionary<GeneDef, bool> geneShown = new Dictionary<GeneDef, bool>();
        private static readonly Dictionary<GeneCategoryDef, List<GeneDef>> categoricalGeneDefs = new Dictionary<GeneCategoryDef, List<GeneDef>>();
        private static readonly List<GeneCategoryDef> geneCategories = new List<GeneCategoryDef>();
        private void CacheGenes()
        {
            _geneDefs.Clear();
            geneShown.Clear();
            geneCategories.Clear();
            categoricalGeneDefs.Clear();
            _geneDefs.AddRange(GeneUtility.GenesInOrder);
            _geneDefs.SortGeneDefs();
            geneCategories.AddRange(DefDatabase<GeneCategoryDef>.AllDefs);
            geneCategories.SortBy<GeneCategoryDef, float>((GeneCategoryDef x) => 0f - x.displayPriorityInXenotype);
            foreach (GeneCategoryDef geneCategory in geneCategories)
            {
                categoricalGeneDefs.Add(geneCategory, new List<GeneDef>());
            }
            foreach (GeneDef geneDef in _geneDefs)
            {
*//*                if ((!searchEmpty && !geneDef.label.ToLower().Contains(searchValue.ToLower())) || (geneDef.biostatArc > 0 && !RIMSPR_DefOfs.RIMSPR_researchArchogeneticEngineering.IsFinished))
                {
                    geneShown.Add(geneDef, value: false);
                }
                else
                {
                    geneShown.Add(geneDef, value: true);
                }*//*
                categoricalGeneDefs[geneDef.displayCategory].Add(geneDef);
            }
        }*/

        private void DrawGenesStats(Rect rect)
        {
            // Example implementation for gene values layout
            float tableHeight = BiostatsTable.HeightForBiostats(alwaysUseFullBiostatsTableHeight ? 1 : archites);
            Rect tableRect = new Rect(rect.x + Margin, rect.y, rect.width - Margin * 2f, tableHeight);
            BiostatsTable.Draw(tableRect, geneComplexitry, metabolism, archites, true, ignoreRestrictions, maxGeneComplexitry);
        }

        protected virtual void PostXenotypeOnGUI(float curX, float curY)
        {
        }

        protected virtual void DrawBottomButtons(Rect rect)
        {
            if (Widgets.ButtonText(new Rect(rect.x, rect.y, ButtonSize.x, ButtonSize.y), "Close".Translate()))
            {
                Close();
            }
        }
    }
}
