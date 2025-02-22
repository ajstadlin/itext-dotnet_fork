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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>Implements lineTo(L) attribute of SVG's path element</summary>
    public class LineTo : AbstractPathShape {
//\cond DO_NOT_DOCUMENT
        internal const int ARGUMENT_SIZE = 2;
//\endcond

        public LineTo()
            : this(false) {
        }

        public LineTo(bool relative)
            : base(relative) {
        }

        public override void Draw() {
            float x = ParseHorizontalLength(coordinates[0]);
            float y = ParseVerticalLength(coordinates[1]);
            context.GetCurrentCanvas().LineTo(x, y);
        }

        public override void SetCoordinates(String[] inputCoordinates, Point startPoint) {
            if (inputCoordinates.Length != ARGUMENT_SIZE) {
                throw new ArgumentException(MessageFormatUtil.Format(SvgExceptionMessageConstant.LINE_TO_EXPECTS_FOLLOWING_PARAMETERS_GOT_0
                    , JavaUtil.ArraysToString(inputCoordinates)));
            }
            this.coordinates = new String[] { inputCoordinates[0], inputCoordinates[1] };
            if (IsRelative()) {
                this.coordinates = copier.MakeCoordinatesAbsolute(coordinates, new double[] { startPoint.GetX(), startPoint
                    .GetY() });
            }
        }
    }
}
