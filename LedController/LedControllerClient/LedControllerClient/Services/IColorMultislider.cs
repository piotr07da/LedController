using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace LedControllerClient.Services
{
    public interface IColorMultislider
    {
        /// <summary>
        /// Gets all markers.
        /// </summary>
        /// <returns></returns>
        List<SCSColorMarker> GetMarkers();

        /// <summary>
        /// Adds color marker at the end of marker list.
        /// </summary>
        void AddMarker();

        /// <summary>
        /// Removes color marker at the end of marker list.
        /// </summary>
        void RemoveMarker();

        /// <summary>
        /// Sets color marker color.
        /// </summary>
        /// <param name="markerIndex">Index of color marker.</param>
        /// <param name="color">Color.</param>
        void SetMarkerColor(int markerIndex, Color color);

        /// <summary>
        /// Occures on color marker selection action.
        /// </summary>
        event Action<ColorMultisliderEventArgs> MarkerSelected;

        /// <summary>
        /// Occures on color marker move action.
        /// </summary>
        event Action<ColorMultisliderEventArgs> MarkerMoved;
    }
}
