using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 配置信息 </summary>
    public class GameConfCfg : ConfigBase
    {
        /// <summary> 玩家子弹数 </summary>
        public int GameZiDan { get; private set; }

        /// <summary> 换弹时间 </summary>
        public int HuanDanTime { get; private set; }

        /// <summary> 怪物基础血量 </summary>
        public int EnemyHp { get; private set; }

        /// <summary> 怪物刷礼加成 </summary>
        public int EnemyAddHp { get; private set; }

        /// <summary> 主播基础攻击力 </summary>
        public int PlayerAtk { get; private set; }

        /// <summary> 玩家基础攻击力 </summary>
        public int EnemyAtk { get; private set; }

        /// <summary> 怪物刷礼物最大血量上限 </summary>
        public int EnemyMaxHp { get; private set; }

        /// <summary> 主播基础血量 </summary>
        public int PlayerMaxHp { get; private set; }

        /// <summary> 怪物放大系数（百分比） </summary>
        public int EnemyScale { get; private set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 2:{
                        GameZiDan = stream.ReadInt();
                        break;
                    }
                    case 3:{
                        HuanDanTime = stream.ReadInt();
                        break;
                    }
                    case 4:{
                        EnemyHp = stream.ReadInt();
                        break;
                    }
                    case 5:{
                        EnemyAddHp = stream.ReadInt();
                        break;
                    }
                    case 6:{
                        PlayerAtk = stream.ReadInt();
                        break;
                    }
                    case 7:{
                        EnemyAtk = stream.ReadInt();
                        break;
                    }
                    case 8:{
                        EnemyMaxHp = stream.ReadInt();
                        break;
                    }
                    case 9:{
                        PlayerMaxHp = stream.ReadInt();
                        break;
                    }
                    case 10:{
                        EnemyScale = stream.ReadInt();
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

            fieldCount += buffer.Write(2, GameZiDan);
            fieldCount += buffer.Write(3, HuanDanTime);
            fieldCount += buffer.Write(4, EnemyHp);
            fieldCount += buffer.Write(5, EnemyAddHp);
            fieldCount += buffer.Write(6, PlayerAtk);
            fieldCount += buffer.Write(7, EnemyAtk);
            fieldCount += buffer.Write(8, EnemyMaxHp);
            fieldCount += buffer.Write(9, PlayerMaxHp);
            fieldCount += buffer.Write(10, EnemyScale);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}