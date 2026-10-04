using System;
using System.Windows;
using FloatingB.Core.Interfaces;
using FloatingB.Core.Models;

namespace FloatingB.Infrastructure.Windows
{
    public class WindowPositionService : IWindowPositionService
    {
        public void PositionAtEdge(
            Window window,
            ScreenEdge edge)
        {
            var workArea = SystemParameters.WorkArea;

            switch (edge)
            {
                case ScreenEdge.Left:

                    window.Left = workArea.Left;

                    KeepVerticalPositionInsideScreen(window);

                    break;

                case ScreenEdge.Right:

                    window.Left =
                        workArea.Right - window.Width;

                    KeepVerticalPositionInsideScreen(window);

                    break;

                case ScreenEdge.Top:

                    window.Top = workArea.Top;

                    KeepHorizontalPositionInsideScreen(window);

                    break;
            }
        }

        public void SnapToNearestEdge(Window window)
        {
            var workArea = SystemParameters.WorkArea;

            double distanceToLeft =
                Math.Abs(
                    window.Left -
                    workArea.Left);

            double distanceToRight =
                Math.Abs(
                    window.Left -
                    (workArea.Right - window.Width));

            double distanceToTop =
                Math.Abs(
                    window.Top -
                    workArea.Top);

            /*
             * Bottom is intentionally NOT included.
             *
             * The control can only snap to:
             * Left
             * Right
             * Top
             */

            double minimumDistance =
                Math.Min(
                    distanceToLeft,
                    Math.Min(
                        distanceToRight,
                        distanceToTop));

            if (minimumDistance == distanceToLeft)
            {
                PositionAtEdge(
                    window,
                    ScreenEdge.Left);
            }
            else if (minimumDistance == distanceToRight)
            {
                PositionAtEdge(
                    window,
                    ScreenEdge.Right);
            }
            else
            {
                PositionAtEdge(
                    window,
                    ScreenEdge.Top);
            }
        }

        private void KeepVerticalPositionInsideScreen(
            Window window)
        {
            var workArea = SystemParameters.WorkArea;

            double minimumTop =
                workArea.Top;

            double maximumTop =
                workArea.Bottom -
                window.Height;

            if (window.Top < minimumTop)
            {
                window.Top = minimumTop;
            }

            if (window.Top > maximumTop)
            {
                window.Top = maximumTop;
            }
        }

        private void KeepHorizontalPositionInsideScreen(
            Window window)
        {
            var workArea = SystemParameters.WorkArea;

            double minimumLeft =
                workArea.Left;

            double maximumLeft =
                workArea.Right -
                window.Width;

            if (window.Left < minimumLeft)
            {
                window.Left = minimumLeft;
            }

            if (window.Left > maximumLeft)
            {
                window.Left = maximumLeft;
            }
        }
    }
}