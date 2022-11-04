#ifndef __XIULIANCONFCFG_H__
#define __XIULIANCONFCFG_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 配置信息
class XiuLianConfCfg : public ConfigBase
{
private:
    typedef ConfigBase Super;
public:
    // 等级
    int32_t LevelId;
    // 阶级名称
    std::string Name;
    // 升级所需经验
    int32_t LevelUp;
    // 技能权限
    int32_t SkillLImit;
    // 对应的角色名称
    std::string RoleName;
    // 成功概率（百分制）
    int32_t LevelUpPr;
    // 特效名称(突破)
    std::string UpEffectName;
    // 战斗场景最大血量
    int32_t FightMaxHp;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 1: {
                    LevelId = stream->ReadInt();
                    break;
                }
                case 2: {
                    Name = stream->ReadString();
                    break;
                }
                case 3: {
                    LevelUp = stream->ReadInt();
                    break;
                }
                case 4: {
                    SkillLImit = stream->ReadInt();
                    break;
                }
                case 5: {
                    RoleName = stream->ReadString();
                    break;
                }
                case 6: {
                    LevelUpPr = stream->ReadInt();
                    break;
                }
                case 7: {
                    UpEffectName = stream->ReadString();
                    break;
                }
                case 8: {
                    FightMaxHp = stream->ReadInt();
                    break;
                }
                default:{
                    stream->ReadUnknow(tagAndType);
                    break;
                }
            }
        }
    }

    void Encode(ProtoStream *stream) override {
        Super::Encode(stream);
        int fieldCount = 0;
        int pos = stream->GetPosition();

        stream->WriteFixedShort(0);

        fieldCount += stream->Write(1, LevelId);
        fieldCount += stream->Write(2, Name);
        fieldCount += stream->Write(3, LevelUp);
        fieldCount += stream->Write(4, SkillLImit);
        fieldCount += stream->Write(5, RoleName);
        fieldCount += stream->Write(6, LevelUpPr);
        fieldCount += stream->Write(7, UpEffectName);
        fieldCount += stream->Write(8, FightMaxHp);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__XIULIANCONFCFG_H__
