using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 配置信息 </summary>
    public class GameConfCfg : ConfigBase
    {
        /// <summary> 僵尸攻击 </summary>
        public int ZombiesAttack { get; private set; }

        /// <summary> 僵尸血量 </summary>
        public int ZombesHp { get; private set; }

        /// <summary> 僵尸移速 </summary>
        public int ZombesSpeed { get; private set; }

        /// <summary> 对应礼物名称 </summary>
        public string GfitName { get; private set; }

        /// <summary> 僵尸名字 </summary>
        public string ZombiesName { get; private set; }

        /// <summary> 僵尸大小 </summary>
        public string zombiesSize { get; private set; }

        /// <summary> 僵尸等级 </summary>
        public int zombiesLv { get; private set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 2:{
                        ZombiesAttack = stream.ReadInt();
                        break;
                    }
                    case 3:{
                        ZombesHp = stream.ReadInt();
                        break;
                    }
                    case 4:{
                        ZombesSpeed = stream.ReadInt();
                        break;
                    }
                    case 5:{
                        GfitName = stream.ReadString();
                        break;
                    }
                    case 6:{
                        ZombiesName = stream.ReadString();
                        break;
                    }
                    case 7:{
                        zombiesSize = stream.ReadString();
                        break;
                    }
                    case 8:{
                        zombiesLv = stream.ReadInt();
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

            fieldCount += buffer.Write(2, ZombiesAttack);
            fieldCount += buffer.Write(3, ZombesHp);
            fieldCount += buffer.Write(4, ZombesSpeed);
            fieldCount += buffer.Write(5, GfitName);
            fieldCount += buffer.Write(6, ZombiesName);
            fieldCount += buffer.Write(7, zombiesSize);
            fieldCount += buffer.Write(8, zombiesLv);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}