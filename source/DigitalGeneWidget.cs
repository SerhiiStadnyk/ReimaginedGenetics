using ReimaginedFoundation;
using UnityEngine;
using Verse;

namespace ReimaginedGenetics
{
    public class DigitalGeneWidget : GeneWidget
    {
        private DigitalGene _digitalGene;

        public override Rect DefaultRect => new Rect(base.DefaultRect.x, base.DefaultRect.y, base.DefaultRect.width, base.DefaultRect.height + AdditionalHeight);

        private static readonly Color CompletedResearchColor = new Color(0f, 0.251f, 0.251f, 1f);
        private static readonly Color UnfinishedResearchColor = new Color(0f, 0f, 1f, 0.5f);
        private static readonly Color OngoingResearchColor = new Color(1f, 1f, 0f, 1f);


        private static readonly Color ProgressBarFilledColor = new Color(0.2f, 0.8f, 0.85f, 1f);
        private static readonly Color ProgressBarBackgroundColor = new Color(0.142f, 0.142f, 0.142f, 1f);
        private static readonly Color ProgressBarOutlineColor = new Color(0f, 0f, 0f, 1f);

        private const float ProgressBarPadding = 2f;
        private const float AdditionalHeight = 25f;
        private const int ProgressBarOutline = 4;

        public DigitalGeneWidget(DigitalGene digitalGene) : base(digitalGene.Gene)
        {
            _digitalGene = digitalGene;
        }

        protected override void InternalDraw(Rect rect)
        {
            base.InternalDraw(rect);

            rect.yMin = rect.yMax - AdditionalHeight;
            DrawProgressSection(rect);
        }

        protected virtual void DrawProgressSection(Rect rect) 
        {
            rect = rect.ContractedBy(ProgressBarPadding);
            float progress = _digitalGene.Progress / _digitalGene.ResearchProgressCost;
            string text = $"{_digitalGene.Progress} / {_digitalGene.ResearchProgressCost}";
            DrawProgressBar(rect, progress, text, ProgressBarOutline);
        }

        protected virtual void DrawProgressBar(Rect rect, float progress, string text = null, int outline = 0) 
        {
            // Outline of the progress bar
            if (outline > 0)
            {
                Widgets.DrawBoxSolid(rect, ProgressBarOutlineColor);
                rect.position += Vector2.one * (outline / 2);
                rect.size -= Vector2.one * outline;
            }

            // Background of the progress bar
            Widgets.DrawBoxSolid(rect, ProgressBarBackgroundColor);

            // Draw filled section of the progress bar
            Rect progressRect = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(progress), rect.height);
            Widgets.DrawBoxSolid(progressRect, ProgressBarFilledColor);

            // Draw text in the middle
            if (text != null && text != "") 
            {
                using (new GUIAnchorScope(TextAnchor.MiddleCenter))
                {
                    Widgets.Label(rect, $"{_digitalGene.Progress} / {_digitalGene.ResearchProgressCost}");
                }
            }
        }

        protected override void DrawContainer(Rect rect)
        {
            // Draw gene container
            Color boxColor = GetBoxColorForGene();
            Widgets.DrawBoxSolid(rect, boxColor);

            // Draw Outline
            using (new GUIColorScope(OutlineColorUnselected))
            {
                Widgets.DrawBox(rect);
            }
        }

        private Color GetBoxColorForGene()
        {
            return _digitalGene.Done ? CompletedResearchColor
                   : _digitalGene.Progress > 0 ? UnfinishedResearchColor
                   : DefaultBoxColor;
        }
    }
}
