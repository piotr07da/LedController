using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace LedController2Client.Services
{
    public interface IColorMultislider
    {
        /// <summary>
        /// Gets all markers.
        /// </summary>
        /// <returns></returns>
        List<SCSColorMarker> GetMarkers();

        /// <summary>
        /// Set markers.
        /// </summary>
        /// <param name="markers">Markers.</param>
        void SetMarkers(List<SCSColorMarker> markers);

        /// <summary>
        /// Sets color marker color.
        /// </summary>
        /// <param name="markerIndex">Index of color marker.</param>
        /// <param name="color">Color.</param>
        void SetMarkerColor(byte markerIndex, Color color);

        /// <summary>
        /// Set progress.
        /// </summary>
        /// <param name="progress"></param>
        void SetProgress(double progress);

        /// <summary>
        /// Occures on color marker selection action.
        /// </summary>
        event Action<ColorMultisliderEventArgs> MarkerSelected;

        /// <summary>
        /// Occures on color marker move action.
        /// </summary>
        event Action<ColorMultisliderEventArgs> MarkerMoved;

        /// <summary>
        /// Occures on progress marker move action.
        /// </summary>
        event Action<ColorMultisliderEventArgs> ProgressMarkerMoved;
    }
}
