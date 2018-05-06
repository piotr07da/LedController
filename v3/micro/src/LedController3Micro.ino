/*
 * Project LedController3Micro
 * Description:
 * Author: Piotr Bejger
 * Created: 2018-04-19
 */

#include "DnsServiceInfo.h"
#include "DnsManager.h"
#include "TcpClientConnectionsManager.h"
#include "ColorTimeLineController.h"
#include "ColorTimeLineTcpCommunicator.h"
#include "LedHeartBeat.h"

#define ServerPort 999
#define ClientConnectionTimeout 1000 * 60 * 3

DnsManager _dnsManager;
TcpClientConnectionsManager _tcpClientConnectionsManager;
ColorTimeLineController _colorTimeLineController;
ColorTimeLineTcpCommunicator _colorTimeLineTcpCommunicator;
LedHeartBeat _ledHeartBeat;

void setup()
{
  pinMode(D3, OUTPUT);
  digitalWrite(D3, HIGH);

  _dnsManager.AddService(DnsServiceInfo_t("tcp", "ledsrv", ServerPort, "LED"));
  _dnsManager.Start();
  _tcpClientConnectionsManager.Setup(ServerPort, ClientConnectionTimeout);
  _tcpClientConnectionsManager.Start();
  _colorTimeLineController.Start();
  _colorTimeLineTcpCommunicator.Setup(&_tcpClientConnectionsManager, _colorTimeLineController.GetColorTimeLine());
  _colorTimeLineTcpCommunicator.Start();
  _ledHeartBeat.Start();
}

void loop()
{
  _dnsManager.Update();
  _tcpClientConnectionsManager.Update();
  _colorTimeLineController.Update();
  _colorTimeLineTcpCommunicator.Update();
  _ledHeartBeat.Update();
}
