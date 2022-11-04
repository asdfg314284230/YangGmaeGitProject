#ifndef __XIULIANSKILLCFG_H__
#define __XIULIANSKILLCFG_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 配置信息
class XiuLianSkillCfg : public ConfigBase
{
private:
    typedef ConfigBase Super;
public:
    // 技能ID
    int32_t SkillId;
    // 技能名称
    std::string SkillName;
    // 技能CD
    int32_t SkillCd;
    // 技能对于造成的数值（群回复保底500）
    int32_t SkillAtk;
    // 技能类型
    std::string SkillType;
    // 场景类型
    int32_t GameScene;
    // 技能动画
    std::string SkillAni;
    // 技能权限
    int32_t SkillLimit;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 1: {
                    SkillId = stream->ReadInt();
                    break;
                }
                case 2: {
                    SkillName = stream->ReadString();
                    break;
                }
                case 3: {
                    SkillCd = stream->ReadInt();
                    break;
                }
                case 4: {
                    SkillAtk = stream->ReadInt();
                    break;
                }
                case 5: {
                    SkillType = stream->ReadString();
                    break;
                }
                case 6: {
                    GameScene = stream->ReadInt();
                    break;
                }
                case 7: {
                    SkillAni = stream->ReadString();
                    break;
                }
                case 8: {
                    SkillLimit = stream->ReadInt();
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

        fieldCount += stream->Write(1, SkillId);
        fieldCount += stream->Write(2, SkillName);
        fieldCount += stream->Write(3, SkillCd);
        fieldCount += stream->Write(4, SkillAtk);
        fieldCount += stream->Write(5, SkillType);
        fieldCount += stream->Write(6, GameScene);
        fieldCount += stream->Write(7, SkillAni);
        fieldCount += stream->Write(8, SkillLimit);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__XIULIANSKILLCFG_H__
