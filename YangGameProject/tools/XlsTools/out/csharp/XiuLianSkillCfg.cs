using System;
using System.Text;
using System.Collections.Generic;
using Stardom.Core.XProto;

namespace Stardom.Core.Model
{
    /// <summary> 配置信息 </summary>
    public class XiuLianSkillCfg : ConfigBase
    {
        /// <summary> 技能ID </summary>
        public int SkillId { get; private set; }

        /// <summary> 技能名称 </summary>
        public string SkillName { get; private set; }

        /// <summary> 技能CD </summary>
        public int SkillCd { get; private set; }

        /// <summary> 技能对于造成的数值（群回复保底500） </summary>
        public int SkillAtk { get; private set; }

        /// <summary> 技能类型 </summary>
        public string SkillType { get; private set; }

        /// <summary> 场景类型 </summary>
        public int GameScene { get; private set; }

        /// <summary> 技能动画 </summary>
        public string SkillAni { get; private set; }

        /// <summary> 技能权限 </summary>
        public int SkillLimit { get; private set; }

        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 1:{
                        SkillId = stream.ReadInt();
                        break;
                    }
                    case 2:{
                        SkillName = stream.ReadString();
                        break;
                    }
                    case 3:{
                        SkillCd = stream.ReadInt();
                        break;
                    }
                    case 4:{
                        SkillAtk = stream.ReadInt();
                        break;
                    }
                    case 5:{
                        SkillType = stream.ReadString();
                        break;
                    }
                    case 6:{
                        GameScene = stream.ReadInt();
                        break;
                    }
                    case 7:{
                        SkillAni = stream.ReadString();
                        break;
                    }
                    case 8:{
                        SkillLimit = stream.ReadInt();
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

            fieldCount += buffer.Write(1, SkillId);
            fieldCount += buffer.Write(2, SkillName);
            fieldCount += buffer.Write(3, SkillCd);
            fieldCount += buffer.Write(4, SkillAtk);
            fieldCount += buffer.Write(5, SkillType);
            fieldCount += buffer.Write(6, GameScene);
            fieldCount += buffer.Write(7, SkillAni);
            fieldCount += buffer.Write(8, SkillLimit);

            if(fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}