#include <stdint.h>
#include <sstream>

class Message
{
    uint8_t m_type;
    std::string m_content;
    std::size_t m_size;

public:
    Message(uint8_t type, std::string content);
    template <typename... Args>
    Message(uint8_t type, Args &&...args);

    const std::string str() const;
    const std::size_t size() const;

    Message(const Message &) = delete;
    Message &operator=(const Message &) = delete;
};

template <typename... Args>
inline Message::Message(uint8_t type, Args &&...args) : m_type(type)
{
    std::stringstream ss;
    ss << type;
    (ss.write(reinterpret_cast<const char *>(&args), sizeof(args)), ...);
    m_content = ss.str();
}
