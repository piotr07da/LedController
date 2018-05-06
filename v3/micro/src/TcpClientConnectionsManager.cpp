#include "TcpClientConnectionsManager.h"

#include "TcpClientConnection.h"

void TcpClientConnectionsManager::Setup(uint16_t serverPort, uint32_t clientConnectionTimeout)
{
  _server = new TCPServer(serverPort);
  _clientConnectionTimeout = clientConnectionTimeout;
}

void TcpClientConnectionsManager::Start()
{
  _server->begin();
}

void TcpClientConnectionsManager::Update()
{
  DetectNewClient();
  ProcessCurrentClients();
}

std::vector<TcpClientConnection> TcpClientConnectionsManager::GetConnections()
{
  return _connections;
}

void TcpClientConnectionsManager::DetectNewClient()
{
  TCPClient client = _server->available();
  if (client.connected())
  {
    TcpClientConnection cc = TcpClientConnection(client);
    _connections.push_back(cc);
  }
}

void TcpClientConnectionsManager::ProcessCurrentClients()
{
  int32_t size = _connections.size();
  for (int32_t i = 0; i < size; ++i)
  {
    TcpClientConnection cc = _connections[i];
    TCPClient c = cc.GetClient();
    bool removeClientConnection = false;

    if (c.connected())
    {
      if (millis() - cc.GetLastUseTime() > _clientConnectionTimeout)
      {
        removeClientConnection = true;
      }
    }
    else
    {
      removeClientConnection = true;
    }

    if (removeClientConnection)
    {
      c.stop();
      _connections.erase(_connections.begin() + i);
    }
  }
}
