#ifndef __TCP_CLIENT_CONNECTION__
#define __TCP_CLIENT_CONNECTION__

#include "Particle.h"

class TcpClientConnection
{
private:
  TCPClient _client;
  uint32_t _lastUseTime;

public:
  TcpClientConnection(TCPClient client);
  TCPClient GetClient();
  uint32_t GetLastUseTime();
  void RefreshLastUseTime();
};

#endif
