using System;
using System.Collections.Generic;
using System.Text;

namespace AnonSocket.Data
{
    public class PacketBuffer
    {
        private byte[] _buffer;
        private int _index;
        private int _reserve;

        public byte[] Buffer
        {
            get
            {
                var temp = new byte[_index];
                Array.Copy(_buffer, 0, temp, 0, _index);
                return temp;
            }
        }
        public int Index { get => _index; }

        public PacketBuffer(int bufferSize)
        {
            _index = 0;
            _reserve = bufferSize;
            _buffer = new byte[_reserve];
        }
        public void SetBuffer(byte[] buffer)
        {
            _buffer = buffer;
            _index = _buffer.Length;
            _reserve = 0;
        }

        public void WriteBuffer(byte[] buffer, int offset, int length)
        {

            if (_reserve >= length)
            {
                Array.Copy(buffer, offset, _buffer, _index, length);
            }
            else
            {
                int totalSize = _buffer.Length + length - _reserve;
                byte[] tempBuffer = new byte[totalSize];
                Array.Copy(_buffer, 0, tempBuffer, 0, _buffer.Length);
                Array.Copy(buffer, offset, tempBuffer, Index, length);
                _buffer = tempBuffer;
            }
            _index += length;
            _reserve = _buffer.Length - Index;
            var buff = Buffer;

        }
        public void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }
        public void ResetBuffer(int packetHead)
        {
            if (packetHead > _index)
                AnonSocketUtil.Debug("Error:PacketBuffer读取过长！！");
            var length = _buffer.Length;
            var tempBuffer = new byte[length];
            _index -= packetHead;  //1
            Array.Copy(_buffer, packetHead, tempBuffer, 0, _index);

            _buffer = tempBuffer;
            _reserve = _buffer.Length - _index;
        }
    }
}
