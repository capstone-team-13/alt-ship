#include <message.h>

Message::Message(uint8_t type, std::string content) : m_type(type), m_content(content)
{
    std::stringstream ss;
    ss << type << m_content;
    m_content = ss.str();
}

const std::string Message::str() const
{
    return m_content;
}

const std::size_t Message::size() const
{
    return m_content.size() + 1;
}