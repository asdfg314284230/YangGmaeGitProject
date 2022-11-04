using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 配置信息 </summary>
    public class PlantsConfCfg : ConfigBase
    {
        /// <summary> 植物攻击 </summary>
        public int PlantAttack { get; private set; }

        /// <summary> 植物血量 </summary>
        public int PlantsHp { get; private set; }

        /// <summary> 植物攻速移速/每多少秒 </summary>
        public string PlantSpeed { get; private set; }

        /// <summary> 子弹移动速度 </summary>
        public string bulletSpeed { get; private set; }

        /// <summary> 子弹个数 </summary>
        public int bulletNum { get; private set; }

        /// <summary> 植物名字 </summary>
        public string plantName { get; private set; }

        /// <summary> 植物序列号/不可更改 </summary>
        public int plantid { get; private set; }

        /// <summary> 植物类型/不可更改 </summary>
        public int plantType { get; private set; }

        /// <summary> 植物花费 </summary>
        public int plantSuns { get; private set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 2:{
                        PlantAttack = stream.ReadInt();
                        break;
                    }
                    case 3:{
                        PlantsHp = stream.ReadInt();
                        break;
                    }
                    case 4:{
                        PlantSpeed = stream.ReadString();
                        break;
                    }
                    case 5:{
                        bulletSpeed = stream.ReadString();
                        break;
                    }
                    case 6:{
                        bulletNum = stream.ReadInt();
                        break;
                    }
                    case 7:{
                        plantName = stream.ReadString();
                        break;
                    }
                    case 8:{
                        plantid = stream.ReadInt();
                        break;
                    }
                    case 9:{
                        plantType = stream.ReadInt();
                        break;
                    }
                    case 10:{
                        plantSuns = stream.ReadInt();
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

            fieldCount += buffer.Write(2, PlantAttack);
            fieldCount += buffer.Write(3, PlantsHp);
            fieldCount += buffer.Write(4, PlantSpeed);
            fieldCount += buffer.Write(5, bulletSpeed);
            fieldCount += buffer.Write(6, bulletNum);
            fieldCount += buffer.Write(7, plantName);
            fieldCount += buffer.Write(8, plantid);
            fieldCount += buffer.Write(9, plantType);
            fieldCount += buffer.Write(10, plantSuns);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}