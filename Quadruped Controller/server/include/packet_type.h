#include <stdint.h>

enum EventType : uint8_t
{
    CONNECTION_SUCCEED,
    POSITION_UPDATE,
    INVALID_EVENT
};