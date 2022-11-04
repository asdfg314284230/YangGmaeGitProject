using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 配置信息 </summary>
    public class RewardTestCfg : ConfigBase
    {
        /// <summary> 礼物ID </summary>
        public int GiftId { get; private set; }

        /// <summary> 礼物名称 </summary>
        public string GiftName { get; private set; }

        /// <summary> 礼物价值 </summary>
        public int Value { get; private set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 1:{
                        GiftId = stream.ReadInt();
                        break;
                    }
                    case 2:{
                        GiftName = stream.ReadString();
                        break;
                    }
                    case 3:{
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

            fieldCount += buffer.Write(1, GiftId);
            fieldCount += buffer.Write(2, GiftName);
            fieldCount += buffer.Write(3, Value);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}