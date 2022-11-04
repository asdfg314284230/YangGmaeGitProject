#ifndef __REWARDTESTCFG_H__
#define __REWARDTESTCFG_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 配置信息
class RewardTestCfg : public ConfigBase
{
private:
    typedef ConfigBase Super;
public:
    // 礼物ID
    int32_t GiftId;
    // 礼物名称
    std::string GiftName;
    // 礼物价值
    int32_t Value;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 1: {
                    GiftId = stream->ReadInt();
                    break;
                }
                case 2: {
                    GiftName = stream->ReadString();
                    break;
                }
                case 3: {
                    Value = stream->ReadInt();
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

        fieldCount += stream->Write(1, GiftId);
        fieldCount += stream->Write(2, GiftName);
        fieldCount += stream->Write(3, Value);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__REWARDTESTCFG_H__
