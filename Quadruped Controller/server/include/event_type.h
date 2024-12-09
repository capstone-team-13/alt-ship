#pragma once

enum EventType : uint8_t
{
    CONNECTION_SUCCEED,
    POSITION_UPDATE,
    BODY_UPDATE,
    ADD_FORCE,
    INVALID_EVENT
};