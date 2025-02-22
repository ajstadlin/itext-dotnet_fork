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
using System.Collections.Generic;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    public class DocumentRenderer : RootRenderer {
        protected internal Document document;

        protected internal IList<int> wrappedContentPage = new List<int>();

        protected internal TargetCounterHandler targetCounterHandler = new TargetCounterHandler();

        public DocumentRenderer(Document document)
            : this(document, true) {
        }

        public DocumentRenderer(Document document, bool immediateFlush) {
            this.document = document;
            this.immediateFlush = immediateFlush;
            this.modelElement = document;
        }

        /// <summary>Get handler for target-counters.</summary>
        /// <returns>
        /// the
        /// <see cref="TargetCounterHandler"/>
        /// instance
        /// </returns>
        public virtual TargetCounterHandler GetTargetCounterHandler() {
            return targetCounterHandler;
        }

        /// <summary>Indicates if relayout is required for targetCounterHandler.</summary>
        /// <returns>true if relayout is required, false otherwise</returns>
        public virtual bool IsRelayoutRequired() {
            return targetCounterHandler.IsRelayoutRequired();
        }

        public override LayoutArea GetOccupiedArea() {
            throw new InvalidOperationException("Not applicable for DocumentRenderer");
        }

        /// <summary>
        /// For
        /// <see cref="DocumentRenderer"/>
        /// , this has a meaning of the renderer that will be used for relayout.
        /// </summary>
        /// <returns>relayout renderer.</returns>
        public override IRenderer GetNextRenderer() {
            iText.Layout.Renderer.DocumentRenderer renderer = new iText.Layout.Renderer.DocumentRenderer(document, immediateFlush
                );
            renderer.targetCounterHandler = new TargetCounterHandler(targetCounterHandler);
            return renderer;
        }

        protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
            FlushWaitingDrawingElements(false);
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            if (taggingHelper != null) {
                taggingHelper.ReleaseFinishedHints();
            }
            AreaBreak areaBreak = overflowResult != null && overflowResult.GetAreaBreak() != null ? overflowResult.GetAreaBreak
                () : null;
            int currentPageNumber = currentArea == null ? 0 : currentArea.GetPageNumber();
            if (areaBreak != null && areaBreak.GetAreaType() == AreaBreakType.LAST_PAGE) {
                while (currentPageNumber < document.GetPdfDocument().GetNumberOfPages()) {
                    PossiblyFlushPreviousPage(currentPageNumber);
                    currentPageNumber++;
                }
            }
            else {
                PossiblyFlushPreviousPage(currentPageNumber);
                currentPageNumber++;
            }
            PageSize customPageSize = areaBreak != null ? areaBreak.GetPageSize() : null;
            while (document.GetPdfDocument().GetNumberOfPages() >= currentPageNumber && document.GetPdfDocument().GetPage
                (currentPageNumber).IsFlushed()) {
                currentPageNumber++;
            }
            PageSize lastPageSize = EnsureDocumentHasNPages(currentPageNumber, customPageSize);
            if (lastPageSize == null) {
                lastPageSize = new PageSize(document.GetPdfDocument().GetPage(currentPageNumber).GetTrimBox());
            }
            return (currentArea = new RootLayoutArea(currentPageNumber, GetCurrentPageEffectiveArea(lastPageSize)));
        }

        protected internal override void FlushSingleRenderer(IRenderer resultRenderer) {
            LinkRenderToDocument(resultRenderer, document.GetPdfDocument());
            Transform transformProp = resultRenderer.GetProperty<Transform>(Property.TRANSFORM);
            if (!waitingDrawingElements.Contains(resultRenderer)) {
                ProcessWaitingDrawing(resultRenderer, transformProp, waitingDrawingElements);
                if (FloatingHelper.IsRendererFloating(resultRenderer) || transformProp != null) {
                    return;
                }
            }
            // TODO Remove checking occupied area to be not null when DEVSIX-1655 is resolved.
            if (!resultRenderer.IsFlushed() && null != resultRenderer.GetOccupiedArea()) {
                int pageNum = resultRenderer.GetOccupiedArea().GetPageNumber();
                PdfDocument pdfDocument = document.GetPdfDocument();
                EnsureDocumentHasNPages(pageNum, null);
                PdfPage correspondingPage = pdfDocument.GetPage(pageNum);
                if (correspondingPage.IsFlushed()) {
                    throw new PdfException(LayoutExceptionMessageConstant.CANNOT_DRAW_ELEMENTS_ON_ALREADY_FLUSHED_PAGES);
                }
                bool wrapOldContent = pdfDocument.GetReader() != null && pdfDocument.GetWriter() != null && correspondingPage
                    .GetContentStreamCount() > 0 && correspondingPage.GetLastContentStream().GetLength() > 0 && !wrappedContentPage
                    .Contains(pageNum) && pdfDocument.GetNumberOfPages() >= pageNum;
                wrappedContentPage.Add(pageNum);
                if (pdfDocument.IsTagged()) {
                    pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().SetPageForTagging(correspondingPage);
                }
                resultRenderer.Draw(new DrawContext(pdfDocument, new PdfCanvas(correspondingPage, wrapOldContent), pdfDocument
                    .IsTagged()));
            }
        }

        /// <summary>Adds new page with defined page size to PDF document.</summary>
        /// <param name="customPageSize">the size of new page, can be null</param>
        /// <returns>the page size of created page</returns>
        protected internal virtual PageSize AddNewPage(PageSize customPageSize) {
            if (customPageSize != null) {
                document.GetPdfDocument().AddNewPage(customPageSize);
            }
            else {
                document.GetPdfDocument().AddNewPage();
            }
            return customPageSize != null ? customPageSize : document.GetPdfDocument().GetDefaultPageSize();
        }

        /// <summary>Ensures that PDF document has n pages.</summary>
        /// <remarks>
        /// Ensures that PDF document has n pages. If document has fewer pages,
        /// adds new pages by calling
        /// <see cref="AddNewPage(iText.Kernel.Geom.PageSize)"/>
        /// method.
        /// </remarks>
        /// <param name="n">the expected number of pages if document</param>
        /// <param name="customPageSize">the size of created pages, can be null</param>
        /// <returns>the page size of the last created page, or null if no page was created</returns>
        protected internal virtual PageSize EnsureDocumentHasNPages(int n, PageSize customPageSize) {
            PageSize lastPageSize = null;
            while (document.GetPdfDocument().GetNumberOfPages() < n) {
                lastPageSize = AddNewPage(customPageSize);
            }
            return lastPageSize;
        }

        private Rectangle GetCurrentPageEffectiveArea(PageSize pageSize) {
            float leftMargin = (float)GetPropertyAsFloat(Property.MARGIN_LEFT);
            float bottomMargin = (float)GetPropertyAsFloat(Property.MARGIN_BOTTOM);
            float topMargin = (float)GetPropertyAsFloat(Property.MARGIN_TOP);
            float rightMargin = (float)GetPropertyAsFloat(Property.MARGIN_RIGHT);
            return new Rectangle(pageSize.GetLeft() + leftMargin, pageSize.GetBottom() + bottomMargin, pageSize.GetWidth
                () - leftMargin - rightMargin, pageSize.GetHeight() - bottomMargin - topMargin);
        }

        private void PossiblyFlushPreviousPage(int currentPageNumber) {
            if (immediateFlush && currentPageNumber > 1) {
                // We don't flush current page immediately, but only flush previous one
                // because of manipulations with areas in case of keepTogether property
                document.GetPdfDocument().GetPage(currentPageNumber - 1).Flush();
            }
        }
    }
}
