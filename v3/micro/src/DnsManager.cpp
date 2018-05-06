#include "DnsManager.h"

#include <vector>

void DnsManager::AddService(DnsServiceInfo_t serviceInfo)
{
  _serviceInfos.push_back(serviceInfo);
}

void DnsManager::Start()
{
  std::vector<String> subServices;

  if (_mdns.setHostname("led"))
  {
    uint32_t size = _serviceInfos.size();
    for (uint32_t i = 0; i < size; ++i)
    {
      DnsServiceInfo_t srvInfo = _serviceInfos[i];
      _mdns.addService(srvInfo.ProtocolName, srvInfo.ServiceName, srvInfo.Port, srvInfo.InstanceName);
    }
    _mdns.begin();
  }
}

void DnsManager::Update()
{
  _mdns.processQueries();
}
