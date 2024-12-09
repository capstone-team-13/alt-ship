#pragma once

enum EventType : uint8_t
{
    CONNECTION_SUCCEED,
    FL_UPDATE,
    BL_UPDATE,
    BR_UPDATE,
    FR_UPDATE,
    BODY_UPDATE,
    INVALID_EVENT
};