#ifndef __COLOR_TIME_LINE_TCP_COMMUNICATOR__
#define __COLOR_TIME_LINE_TCP_COMMUNICATOR__

#include "TcpClientConnectionsManager.h"
#include "ColorTimeLine.h"
#include <vector>

class ColorTimeLineTcpCommunicator
{
static const uint8_t ReadCycleTimeMessageHeader = 0x01;
static const uint8_t ReadTimeProgressMessageHeader = 0x02;
static const uint8_t ReadColorTimePointsMessageHeader = 0x03;
static const uint8_t ReadColorTimePointMessageHeader = 0x04;
static const uint8_t WriteCycleTimeMessageHeader = 0x71;
static const uint8_t WriteTimeProgressMessageHeader = 0x72;
static const uint8_t WriteColorTimePointColorMessageHeader = 0x73;
static const uint8_t WriteColorTimePointTimeMessageHeader = 0x74;

private:
  TcpClientConnectionsManager* _tcpClientConnectionsManager;
  ColorTimeLine* _colorTimeLine;

public:

  void Setup(TcpClientConnectionsManager* tcpClientConnectionsManager, ColorTimeLine* colorTimeLine);
  void Start();
  void Update();

private:
  void HandleReadCycleTimeMessage(TCPClient client);
  void HandleReadTimeProgressMessage(TCPClient client);
  void HandleReadColorTimePointsMessage(TCPClient client);
  void AppendValueBytes(std::vector<byte>* targetBytes, void* value, uint32_t valueSize);
};

#endif
