#ifndef __DNS_MANAGER__
#define __DNS_MANAGER__

#include "MDNS.h"
#include "DnsServiceInfo.h"

class DnsManager
{
private:
  MDNS _mdns;
  std::vector<DnsServiceInfo_t> _serviceInfos;

public:
  void AddService(DnsServiceInfo_t serviceInfo);
  void Start();
  void Update();
};

#endif
