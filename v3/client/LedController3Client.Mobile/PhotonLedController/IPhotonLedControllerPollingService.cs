using System;
using System.Collections.Generic;
using System.Text;

namespace LedController3Client.Mobile.PhotonLedController
{
    public interface IPhotonLedControllerPollingService
    {
        void Start();
        void Stop();
    }
}
