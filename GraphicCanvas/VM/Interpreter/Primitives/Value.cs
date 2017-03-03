using System;
using System.Text;

using System.Drawing;

namespace GraphicCanvas.VM
{
    struct Value
    {
        public MyValueType type;
        public object data;

        #region Constructors

        public Value(MyValueType type, byte[] data, int startIndex = 0)
        {
            this.type = type;

            switch (type)
            {
                case MyValueType.INTEGER:
                    this.data = BitConverter.ToInt32(data, startIndex);
                    break;
                case MyValueType.COLOR:
                    this.data = Color.FromArgb(BitConverter.ToInt32(data, startIndex));
                    break;
                case MyValueType.STRING:
                    this.data = Encoding.ASCII.GetString(data);
                    break;
                default:
                    this.data = null;
                    break;
            }
        }

        public Value(Color color)
        {
            type = MyValueType.COLOR;
            data = color;
        }

        public Value(int integer)
        {
            type = MyValueType.INTEGER;
            data = integer;
        }

        public Value(string charString)
        {
            type = MyValueType.STRING;
            data = charString;
        }

        #endregion

        public T GetAs<T>()
        {
            if (!(data is T))
            {
                throw new Exception("Type missmatch.");
            }

            return (T)data;
        }
    }
}
