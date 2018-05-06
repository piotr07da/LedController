using System.Net;
using System.Threading;
using Tmds.MDns;

namespace LedController3Client.Communication
{
    public class LedServiceHostProvider
    {
        private ServiceAnnouncement _serviceAnnouncement;

        public IPAddress HostIpAddress
        {
            get
            {
                if (_serviceAnnouncement == null)
                {
                    DiscoverHost();
                }
                return _serviceAnnouncement.Addresses[0];
            }
        }

        public ushort HostPort
        {
            get
            {
                if (_serviceAnnouncement == null)
                {
                    DiscoverHost();
                }
                return _serviceAnnouncement.Port;
            }
        }

        private void DiscoverHost()
        {
            var id = Thread.CurrentThread.ManagedThreadId;

            var serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += ServiceBrowser_ServiceAdded;
            serviceBrowser.StartBrowse("_ledsrv._tcp", false);
            while (_serviceAnnouncement == null)
            {
                Thread.Sleep(10);
            }
        }

        private void ServiceBrowser_ServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            _serviceAnnouncement = e.Announcement;
        }
    }
}
