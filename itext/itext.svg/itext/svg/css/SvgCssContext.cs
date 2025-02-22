/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Css.Impl;

namespace iText.Svg.Css {
    /// <summary>
    /// Context necessary for evaluating certain Css statements whose final values depends on other statements
    /// e.g. relative font-size statements.
    /// </summary>
    public class SvgCssContext : AbstractCssContext {
        /// <summary>The root font size value in pt.</summary>
        private float rootFontSize = SvgStyleResolver.DEFAULT_FONT_SIZE;

        /// <summary>Gets the root font size.</summary>
        /// <returns>the root font size in pt</returns>
        public virtual float GetRootFontSize() {
            return rootFontSize;
        }

        /// <summary>Sets the root font size.</summary>
        /// <param name="fontSizeStr">the new root font size</param>
        public virtual void SetRootFontSize(String fontSizeStr) {
            this.rootFontSize = CssDimensionParsingUtils.ParseAbsoluteFontSize(fontSizeStr);
        }
    }
}
