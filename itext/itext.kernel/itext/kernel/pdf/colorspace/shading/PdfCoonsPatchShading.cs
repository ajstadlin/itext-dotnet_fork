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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// ,
    /// <see cref="AbstractPdfShadingMesh"/>
    /// and
    /// <see cref="AbstractPdfShadingMeshWithFlags"/>
    /// classes and is in charge of Shading Dictionary with Coons Patch mesh type.
    /// </summary>
    /// <remarks>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// ,
    /// <see cref="AbstractPdfShadingMesh"/>
    /// and
    /// <see cref="AbstractPdfShadingMeshWithFlags"/>
    /// classes and is in charge of Shading Dictionary with Coons Patch mesh type.
    /// <para />
    /// This type of shading is constructed from one or more colour patches, each bounded by four cubic Bézier curves.
    /// Degenerate Bézier curves are allowed and are useful for certain graphical effects.
    /// At least one complete patch shall be specified.
    /// <para />
    /// The shape of patch is defined by 12 control points.
    /// <para />
    /// Colours are specified for each corner of the unit square,
    /// and bilinear interpolation is used to fill in colours over the entire unit square.
    /// <para />
    /// Coordinates are mapped from the unit square into a four-sided patch whose sides are not necessarily linear.
    /// The mapping is continuous: the corners of the unit square map to corners of the patch
    /// and the sides of the unit square map to sides of the patch.
    /// <para />
    /// For the format of data stream, that defines patches (see ISO-320001 Table 85).
    /// <para />
    /// If the shading dictionary contains a Function entry, the colour data for each corner of a patch
    /// shall be specified by a single parametric value t rather than by n separate colour components c1...cn.
    /// </remarks>
    public class PdfCoonsPatchShading : AbstractPdfShadingMeshWithFlags {
        /// <summary>
        /// Creates the new instance of the class from the existing
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </summary>
        /// <param name="pdfStream">
        /// from which this
        /// <see cref="PdfCoonsPatchShading"/>
        /// will be created
        /// </param>
        public PdfCoonsPatchShading(PdfStream pdfStream)
            : base(pdfStream) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="bitsPerCoordinate">
        /// the number of bits used to represent each vertex coordinate.
        /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32
        /// </param>
        /// <param name="bitsPerComponent">
        /// the number of bits used to represent each colour component.
        /// The value shall be 1, 2, 4, 8, 12, or 16
        /// </param>
        /// <param name="bitsPerFlag">
        /// the number of bits used to represent the edge flag for each vertex.
        /// The value of BitsPerFlag shall be 2, 4, or 8,
        /// but only the least significant 2 bits in each flag value shall be used.
        /// The value for the edge flag shall be 0, 1, 2 or 3
        /// </param>
        /// <param name="decode">
        /// the
        /// <c>int[]</c>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present
        /// </param>
        public PdfCoonsPatchShading(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag
            , float[] decode)
            : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="bitsPerCoordinate">
        /// the number of bits used to represent each vertex coordinate.
        /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32
        /// </param>
        /// <param name="bitsPerComponent">
        /// the number of bits used to represent each colour component.
        /// The value shall be 1, 2, 4, 8, 12, or 16
        /// </param>
        /// <param name="bitsPerFlag">
        /// the number of bits used to represent the edge flag for each vertex.
        /// The value of BitsPerFlag shall be 2, 4, or 8,
        /// but only the least significant 2 bits in each flag value shall be used.
        /// The value for the edge flag shall be 0, 1, 2 or 3
        /// </param>
        /// <param name="decode">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present
        /// </param>
        public PdfCoonsPatchShading(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent, int bitsPerFlag
            , PdfArray decode)
            : base(new PdfStream(), ShadingType.COONS_PATCH_MESH, cs) {
            SetBitsPerCoordinate(bitsPerCoordinate);
            SetBitsPerComponent(bitsPerComponent);
            SetBitsPerFlag(bitsPerFlag);
            SetDecode(decode);
        }
    }
}
