#include "TcpClientConnection.h"

TcpClientConnection::TcpClientConnection(TCPClient client)
{
  _client = client;
  RefreshLastUseTime();
}

TCPClient TcpClientConnection::GetClient()
{
  return _client;
}

uint32_t TcpClientConnection::GetLastUseTime()
{
  return _lastUseTime;
}

void TcpClientConnection::RefreshLastUseTime()
{
  _lastUseTime = millis();
}
