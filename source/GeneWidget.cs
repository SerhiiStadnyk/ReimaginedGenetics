using ReimaginedFoundation;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReimaginedGenetics
{
    public class GeneWidget : IDraweable
    {
        protected GeneDef _geneDef;

        protected static readonly Color DefaultBoxColor = new Color(0.251f, 0.255f, 0.255f, 1f);
        protected static readonly Color OutlineColorUnselected = new Color(1f, 1f, 1f, 0.1f);

        protected Rect? _assignedRect;

        public virtual Rect DefaultRect => new Rect(0, 0, 46f + GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y + 8f);

        public Rect Rect
        {
            get => _assignedRect ?? DefaultRect;
            set => _assignedRect = value;
        }

        public GeneDef GeneDef => _geneDef;

        public GeneWidget(GeneDef geneDef)
        {
            _geneDef = geneDef;
        }

        public virtual void Draw(Rect rect)
        {
            InternalDraw(rect);
            DrawButton(rect);
        }

        protected virtual void InternalDraw(Rect rect) 
        {
            DrawContainer(rect);

            float xPos = rect.x + 4f;
            DrawaBiostat(rect, ref xPos);
            Rect currentRect = rect;
            currentRect.x = xPos;

            DrawGene(currentRect);
            DrawInfoCard(rect);
        }

        protected virtual void DrawButton(Rect rect)
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
        }

        protected virtual void DrawInfoCard(Rect rect)
        {
            Widgets.InfoCardButton(rect.xMax - 24f, rect.y + 2f, _geneDef);
        }

        protected virtual void DrawContainer(Rect rect) 
        {
            // Draw gene container
            Widgets.DrawHighlight(rect);

            // Draw Outline
            using (new GUIColorScope(OutlineColorUnselected))
            {
                Widgets.DrawBox(rect);
            }
        }

        protected virtual void DrawaBiostat(Rect rect, ref float xPos)
        {
            GeneUIUtility.DrawBiostats(_geneDef.biostatCpx, _geneDef.biostatMet, _geneDef.biostatArc, ref xPos, rect.y, 4f);
        }

        protected virtual void DrawGene(Rect rect) 
        {
            // Calculate geneRect for centered placement
            float geneRectWidth = GeneCreationDialogBase.GeneSize.x;
            float geneRectHeight = GeneCreationDialogBase.GeneSize.y;

            // Center geneRect within rect
            Rect geneRect = new Rect(
                rect.x,
                rect.yMin + 4f,
                geneRectWidth,
                geneRectHeight
            );

            GeneUIUtility.DrawGeneDef(_geneDef, geneRect, GeneType.Xenogene, null, doBackground: false, clickable: false, false);
        }
    }
}
