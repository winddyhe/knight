//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework.Graphics
{
    [System.Serializable]
    public class TextLayout
    {
        public string                   Text;
        public Vector2                  Extents;

        public FontData                 FontData                = FontData.defaultFontData;
        public Color                    FontColor;
        public TextGenerator            TextGenerator;
        public VertexElement            VertexElement;

        private TextGenerationSettings  mTextGeneratorSettings;

        public TextLayout()
        {
            this.VertexElement = new VertexElement();
            mTextGeneratorSettings = new TextGenerationSettings();
        }

        public void GeneratorText()
        {
            this.GetGenerationSettings();

            this.TextGenerator = this.TextGenerator ?? (!string.IsNullOrEmpty(this.Text) && this.Text.Length != 0 ? new TextGenerator(this.Text.Length) : new TextGenerator());
            this.TextGenerator.Invalidate();
            this.TextGenerator.Populate(this.Text, mTextGeneratorSettings);

            this.VertexElement.Clear();

            var rTextVertices = this.TextGenerator.verts;
            for (int i = 0; i < rTextVertices.Count; i++)
            {
                this.VertexElement.AddVert(rTextVertices[i].position, rTextVertices[i].color, rTextVertices[i].uv0);
                if (i % 4 == 0)
                {
                    int nStartIndex = i;
                    this.VertexElement.AddTriangle(nStartIndex, nStartIndex + 1, nStartIndex + 2);
                    this.VertexElement.AddTriangle(nStartIndex + 2, nStartIndex + 3, nStartIndex);
                }
            }
        }

        public void SetMeshVertices(Mesh rMesh)
        {
            this.VertexElement.SetMeshVertices(rMesh);
        }

        public void SetMeshIndices(Mesh rMesh)
        {
            this.VertexElement.SetMeshIndices(rMesh);
        }

        private void GetGenerationSettings()
        {
            mTextGeneratorSettings.generationExtents = this.Extents;
            if (this.FontData != null && this.FontData.font != null && this.FontData.font.dynamic)
            {
                mTextGeneratorSettings.fontSize = FontData.fontSize;
                mTextGeneratorSettings.resizeTextMinSize = this.FontData.minSize;
                mTextGeneratorSettings.resizeTextMaxSize = this.FontData.maxSize;
            }

            // Other settings
            mTextGeneratorSettings.textAnchor = this.FontData.alignment;
            mTextGeneratorSettings.scaleFactor = 1;
            mTextGeneratorSettings.color = this.FontColor;
            mTextGeneratorSettings.font = this.FontData.font;
            mTextGeneratorSettings.pivot = Vector2.zero;
            mTextGeneratorSettings.richText = this.FontData.richText;
            mTextGeneratorSettings.lineSpacing = this.FontData.lineSpacing;
            mTextGeneratorSettings.fontStyle = this.FontData.fontStyle;
            mTextGeneratorSettings.resizeTextForBestFit = this.FontData.bestFit;
            mTextGeneratorSettings.updateBounds = false;
            mTextGeneratorSettings.horizontalOverflow = this.FontData.horizontalOverflow;
            mTextGeneratorSettings.verticalOverflow = this.FontData.verticalOverflow;
        }
    }
}