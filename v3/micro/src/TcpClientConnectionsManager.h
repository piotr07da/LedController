#ifndef __TCP_CLIENT_CONNECTIONS_MANAGER__
#define __TCP_CLIENT_CONNECTIONS_MANAGER__

#include <vector>
#include "Particle.h"
#include "TcpClientConnection.h"

class TcpClientConnectionsManager
{
private:
  TCPServer* _server;
  std::vector<TcpClientConnection> _connections;
  uint32_t _clientConnectionTimeout;

public:
  void Setup(uint16_t serverPort, uint32_t clientConnectionTimeout);
  void Start();
  void Update();
  std::vector<TcpClientConnection> GetConnections();

private:
  void DetectNewClient();
  void ProcessCurrentClients();
};

#endif
