using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 配置信息 </summary>
    public class XiuLianConfCfg : ConfigBase
    {
        /// <summary> 等级 </summary>
        public int LevelId { get; private set; }

        /// <summary> 阶级名称 </summary>
        public string Name { get; private set; }

        /// <summary> 升级所需经验 </summary>
        public int LevelUp { get; private set; }

        /// <summary> 技能权限 </summary>
        public int SkillLImit { get; private set; }

        /// <summary> 对应的角色名称 </summary>
        public string RoleName { get; private set; }

        /// <summary> 成功概率（百分制） </summary>
        public int LevelUpPr { get; private set; }

        /// <summary> 特效名称(突破) </summary>
        public string UpEffectName { get; private set; }

        /// <summary> 战斗场景最大血量 </summary>
        public int FightMaxHp { get; private set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 1:{
                        LevelId = stream.ReadInt();
                        break;
                    }
                    case 2:{
                        Name = stream.ReadString();
                        break;
                    }
                    case 3:{
                        LevelUp = stream.ReadInt();
                        break;
                    }
                    case 4:{
                        SkillLImit = stream.ReadInt();
                        break;
                    }
                    case 5:{
                        RoleName = stream.ReadString();
                        break;
                    }
                    case 6:{
                        LevelUpPr = stream.ReadInt();
                        break;
                    }
                    case 7:{
                        UpEffectName = stream.ReadString();
                        break;
                    }
                    case 8:{
                        FightMaxHp = stream.ReadInt();
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

            fieldCount += buffer.Write(1, LevelId);
            fieldCount += buffer.Write(2, Name);
            fieldCount += buffer.Write(3, LevelUp);
            fieldCount += buffer.Write(4, SkillLImit);
            fieldCount += buffer.Write(5, RoleName);
            fieldCount += buffer.Write(6, LevelUpPr);
            fieldCount += buffer.Write(7, UpEffectName);
            fieldCount += buffer.Write(8, FightMaxHp);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}