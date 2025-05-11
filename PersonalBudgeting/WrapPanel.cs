using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace PersonalBudgeting
{
    /// <summary>
    /// A panel that positions child elements in sequential position from left to right,
    /// breaking content to the next line at the edge of the containing box. 
    /// Subsequent ordering happens sequentially from top to bottom or right to left, 
    /// depending on the value of the Orientation property.
    /// </summary>
    public class WrapPanel : Panel
    {
        /// <summary>
        /// Defines the <see cref="Orientation"/> property.
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<WrapPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

        /// <summary>
        /// Defines the <see cref="Spacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> SpacingProperty =
            AvaloniaProperty.Register<WrapPanel, double>(nameof(Spacing), 0.0);

        /// <summary>
        /// Gets or sets the direction in which child elements are arranged.
        /// </summary>
        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the uniform spacing between elements.
        /// </summary>
        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        /// <summary>
        /// Measures the child elements of a <see cref="WrapPanel"/> in anticipation of arranging them during the
        /// <see cref="ArrangeOverride"/> pass.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that the panel determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double spacing = Spacing;
            var orientation = Orientation;
            var isHorizontal = orientation == Orientation.Horizontal;

            double itemWidth = 0;
            double itemHeight = 0;
            double lineWidth = 0;
            double lineHeight = 0;
            double totalWidth = 0;
            double totalHeight = 0;

            foreach (var child in Children)
            {
                child.Measure(availableSize);
                var childSize = child.DesiredSize;

                if (isHorizontal)
                {
                    itemWidth = childSize.Width;
                    itemHeight = childSize.Height;

                    if (lineWidth + itemWidth > availableSize.Width)
                    {
                        // Move to next line
                        totalWidth = Math.Max(lineWidth - spacing, totalWidth);
                        totalHeight += lineHeight + spacing;
                        lineWidth = itemWidth + spacing;
                        lineHeight = itemHeight;
                    }
                    else
                    {
                        // Stay on current line
                        lineWidth += itemWidth + spacing;
                        lineHeight = Math.Max(lineHeight, itemHeight);
                    }
                }
                else // Vertical
                {
                    itemWidth = childSize.Width;
                    itemHeight = childSize.Height;

                    if (lineHeight + itemHeight > availableSize.Height)
                    {
                        // Move to next column
                        totalHeight = Math.Max(lineHeight - spacing, totalHeight);
                        totalWidth += lineWidth + spacing;
                        lineHeight = itemHeight + spacing;
                        lineWidth = itemWidth;
                    }
                    else
                    {
                        // Stay in current column
                        lineHeight += itemHeight + spacing;
                        lineWidth = Math.Max(lineWidth, itemWidth);
                    }
                }
            }

            // Add the last line/column size
            if (isHorizontal)
            {
                totalWidth = Math.Max(lineWidth - spacing, totalWidth);
                totalHeight += lineHeight;
            }
            else
            {
                totalHeight = Math.Max(lineHeight - spacing, totalHeight);
                totalWidth += lineWidth;
            }

            return new Size(
                Math.Min(availableSize.Width, totalWidth), 
                Math.Min(availableSize.Height, totalHeight));
        }

        /// <summary>
        /// Arranges the content of a <see cref="WrapPanel"/> element.
        /// </summary>
        /// <param name="finalSize">The size that this element should use to arrange its child elements.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double spacing = Spacing;
            var orientation = Orientation;
            bool isHorizontal = orientation == Orientation.Horizontal;

            double itemWidth = 0;
            double itemHeight = 0;
            double lineWidth = 0;
            double lineHeight = 0;

            double currentX = 0;
            double currentY = 0;

            foreach (var child in Children)
            {
                var childSize = child.DesiredSize;

                if (isHorizontal)
                {
                    itemWidth = childSize.Width;
                    itemHeight = childSize.Height;

                    if (currentX + itemWidth > finalSize.Width)
                    {
                        // Move to the next line
                        currentY += lineHeight + spacing;
                        currentX = 0;
                        lineHeight = 0;
                    }

                    child.Arrange(new Rect(currentX, currentY, itemWidth, itemHeight));
                    currentX += itemWidth + spacing;
                    lineHeight = Math.Max(lineHeight, itemHeight);
                }
                else // Vertical
                {
                    itemWidth = childSize.Width;
                    itemHeight = childSize.Height;

                    if (currentY + itemHeight > finalSize.Height)
                    {
                        // Move to the next column
                        currentX += lineWidth + spacing;
                        currentY = 0;
                        lineWidth = 0;
                    }

                    child.Arrange(new Rect(currentX, currentY, itemWidth, itemHeight));
                    currentY += itemHeight + spacing;
                    lineWidth = Math.Max(lineWidth, itemWidth);
                }
            }

            return finalSize;
        }
    }
} 
