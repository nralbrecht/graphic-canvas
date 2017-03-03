using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicCanvas.VM
{
    enum Instruction
    {
        READ = 0x00,
        WRITE = 0x01,
        DUP = 0x02,

        INTEGER_LITERAL = 0x10,
        COLOR_LITERAL = 0x11,
        STRING_LITERAL = 0x12,
        
        FILL = 0x20,
        SET_PIXEL = 0x21,
        SET_RECT = 0x22,
        WRITE_TEXT = 0x23,

        RGB = 0x30,
        CMYK = 0x31,
        HSV = 0x32,
        LAB = 0x33,

        GET_PIXEL = 0x40,
        GET_NOISE = 0x41,
        GET_WIDTH = 0x42,
        GET_HEIGHT = 0x43,
        GET_RAND = 0x44 
    }
}
