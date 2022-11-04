using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 键值对 </summary>
    public class KeyValue : Message
    {
        /// <summary> Key </summary>
        public int Key { get; set; }

        /// <summary> Value </summary>
        public int Value { get; set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 1:{
                        Key = stream.ReadInt();
                        break;
                    }
                    case 2:{
                        Value = stream.ReadInt();
                        break;
                    }
                    default:{
                        stream.ReadUnknow(tagAndType);
                        break;
                    }
                }
            }
        }

        public override void Encode(ProtoStream buffer)
        {
            base.Encode(buffer);

            int fieldCount = 0;
            int pos = buffer.Position;

            buffer.WriteFixedShort(0);

            fieldCount += buffer.Write(1, Key);
            fieldCount += buffer.Write(2, Value);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}