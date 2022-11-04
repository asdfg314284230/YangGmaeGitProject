#ifndef __GAMECONFCFG_H__
#define __GAMECONFCFG_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 配置信息
class GameConfCfg : public ConfigBase
{
private:
    typedef ConfigBase Super;
public:
    // 僵尸攻击
    int32_t ZombiesAttack;
    // 僵尸血量
    int32_t ZombesHp;
    // 僵尸移速
    int32_t ZombesSpeed;
    // 对应礼物名称
    std::string GfitName;
    // 僵尸名字
    std::string ZombiesName;
    // 僵尸大小
    std::string zombiesSize;
    // 僵尸等级
    int32_t zombiesLv;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 2: {
                    ZombiesAttack = stream->ReadInt();
                    break;
                }
                case 3: {
                    ZombesHp = stream->ReadInt();
                    break;
                }
                case 4: {
                    ZombesSpeed = stream->ReadInt();
                    break;
                }
                case 5: {
                    GfitName = stream->ReadString();
                    break;
                }
                case 6: {
                    ZombiesName = stream->ReadString();
                    break;
                }
                case 7: {
                    zombiesSize = stream->ReadString();
                    break;
                }
                case 8: {
                    zombiesLv = stream->ReadInt();
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

        fieldCount += stream->Write(2, ZombiesAttack);
        fieldCount += stream->Write(3, ZombesHp);
        fieldCount += stream->Write(4, ZombesSpeed);
        fieldCount += stream->Write(5, GfitName);
        fieldCount += stream->Write(6, ZombiesName);
        fieldCount += stream->Write(7, zombiesSize);
        fieldCount += stream->Write(8, zombiesLv);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__GAMECONFCFG_H__
