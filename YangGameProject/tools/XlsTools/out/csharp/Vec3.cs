using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 坐标 </summary>
    public class Vec3 : Message
    {
        /// <summary> A </summary>
        public int A { get; set; }

        /// <summary> B </summary>
        public int B { get; set; }

        /// <summary> C </summary>
        public int C { get; set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 1:{
                        A = stream.ReadInt();
                        break;
                    }
                    case 2:{
                        B = stream.ReadInt();
                        break;
                    }
                    case 3:{
                        C = stream.ReadInt();
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

            fieldCount += buffer.Write(1, A);
            fieldCount += buffer.Write(2, B);
            fieldCount += buffer.Write(3, C);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}