namespace GraphicCanvas.VM
{
    enum TokenType
    {
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN,
        COMMA, SEMICOLON, EQUAL,

        // Literals.
        IDENTIFIER, STRING, NUMBER, COLOR,

        // Keywords.
        VAR,

        EOF
    }
}
