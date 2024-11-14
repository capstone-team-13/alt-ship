#include <stdint.h>
#include <sstream>

namespace MessageType
{
    enum Type
    {
        CONNECT,
        DISCONNECT,
        DATA,
    };
}

class Message
{
    uint8_t m_type;
    std::string m_content;
    std::size_t m_size;

public:
    Message(uint8_t type, std::string content);

    const std::string str() const;
    const std::size_t size() const;

    Message(const Message &) = delete;
    Message &operator=(const Message &) = delete;
};