#include "ColorTimeLineTcpCommunicator.h"

#include <vector>
#include "Particle.h"
#include "Color.h"
#include "ColorTimePoint.h"

void ColorTimeLineTcpCommunicator::Setup(TcpClientConnectionsManager* tcpClientConnectionsManager, ColorTimeLine* colorTimeLine)
{
  _tcpClientConnectionsManager = tcpClientConnectionsManager;
  _colorTimeLine = colorTimeLine;
}

void ColorTimeLineTcpCommunicator::Start()
{

}

void ColorTimeLineTcpCommunicator::Update()
{
  std::vector<TcpClientConnection> ccs = _tcpClientConnectionsManager->GetConnections();
  int32_t size = ccs.size();
  for (int32_t i = 0; i < size; ++i)
  {
    TcpClientConnection cc = ccs[i];
    TCPClient c = cc.GetClient();
    if (c.available())
    {
      cc.RefreshLastUseTime();

      while (c.available())
      {
        uint8_t mh = c.read();
        switch (mh)
        {
          case ReadCycleTimeMessageHeader:
            HandleReadCycleTimeMessage(c);
            break;

          case ReadTimeProgressMessageHeader:
            HandleReadTimeProgressMessage(c);
            break;

          case ReadColorTimePointsMessageHeader:
            HandleReadColorTimePointsMessage(c);
            break;

          case ReadColorTimePointMessageHeader:
            break;

          case WriteCycleTimeMessageHeader:
            break;

          case WriteTimeProgressMessageHeader:
            break;

          case WriteColorTimePointColorMessageHeader:
            break;

          case WriteColorTimePointTimeMessageHeader:
            break;
        }
      }

      c.flush();
    }
  }
}

void ColorTimeLineTcpCommunicator::HandleReadCycleTimeMessage(TCPClient client)
{
  std::vector<byte> bytes;
  int32_t cycleTime = _colorTimeLine->GetCycleTime();
  AppendValueBytes(&bytes, (void*)&cycleTime, 4);
  client.write(bytes.data(), 4);
}

void ColorTimeLineTcpCommunicator::HandleReadTimeProgressMessage(TCPClient client)
{
  std::vector<byte> bytes;
  float timeProgress = _colorTimeLine->GetTimeProgress();
  AppendValueBytes(&bytes, (void*)&timeProgress, 4);
  client.write(bytes.data(), 4);
}

void ColorTimeLineTcpCommunicator::HandleReadColorTimePointsMessage(TCPClient client)
{
  std::vector<uint8_t> bytes;
  ColorTimePoint* points = _colorTimeLine->GetPoints();
  uint8_t size = _colorTimeLine->GetPointCount();

  // 1 byte for number of points.
  bytes.push_back(size);

  for (int32_t pIx = 0; pIx < size; ++pIx)
  {
    ColorTimePoint p = points[pIx];
    uint8_t id = p.GetId();
    Color_t c = p.GetColor();
    float t = p.GetTime();

    // 1 byte for Id
    bytes.push_back(id);

    // 3 bytes for Color
    bytes.push_back(c.R);
    bytes.push_back(c.G);
    bytes.push_back(c.B);

    // 4 bytes for Time
    AppendValueBytes(&bytes, (void*)&t, 4);
  }

  client.write(bytes.data(), 1 + size * 8); // 1 byte for number of points + 8 bytes for each point
}

void ColorTimeLineTcpCommunicator::AppendValueBytes(std::vector<byte>* targetBytes, void* value, uint32_t valueSize)
{
  for (uint32_t bIx = 0; bIx < valueSize; ++bIx)
  {
    uint8_t byte = *((uint8_t*)(value + bIx));
    targetBytes->push_back(byte);
  }
}
