#ifndef __DNS_SERVICE_INFO__
#define __DNS_SERVICE_INFO__

struct DnsServiceInfo
{
  String ProtocolName;
  String ServiceName;
  uint16_t Port;
  String InstanceName;

  DnsServiceInfo(String protocolName, String serviceName, uint16_t port, String instanceName)
  {
    ProtocolName = protocolName;
    ServiceName = serviceName;
    Port = port;
    InstanceName = instanceName;
  }
};

typedef DnsServiceInfo DnsServiceInfo_t;

#endif
