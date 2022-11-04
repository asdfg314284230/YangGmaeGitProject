#ifndef __PLANTSCONFCFG_H__
#define __PLANTSCONFCFG_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 配置信息
class PlantsConfCfg : public ConfigBase
{
private:
    typedef ConfigBase Super;
public:
    // 植物攻击
    int32_t PlantAttack;
    // 植物血量
    int32_t PlantsHp;
    // 植物攻速移速/每多少秒
    std::string PlantSpeed;
    // 子弹移动速度
    std::string bulletSpeed;
    // 子弹个数
    int32_t bulletNum;
    // 植物名字
    std::string plantName;
    // 植物序列号/不可更改
    int32_t plantid;
    // 植物类型/不可更改
    int32_t plantType;
    // 植物花费
    int32_t plantSuns;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 2: {
                    PlantAttack = stream->ReadInt();
                    break;
                }
                case 3: {
                    PlantsHp = stream->ReadInt();
                    break;
                }
                case 4: {
                    PlantSpeed = stream->ReadString();
                    break;
                }
                case 5: {
                    bulletSpeed = stream->ReadString();
                    break;
                }
                case 6: {
                    bulletNum = stream->ReadInt();
                    break;
                }
                case 7: {
                    plantName = stream->ReadString();
                    break;
                }
                case 8: {
                    plantid = stream->ReadInt();
                    break;
                }
                case 9: {
                    plantType = stream->ReadInt();
                    break;
                }
                case 10: {
                    plantSuns = stream->ReadInt();
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

        fieldCount += stream->Write(2, PlantAttack);
        fieldCount += stream->Write(3, PlantsHp);
        fieldCount += stream->Write(4, PlantSpeed);
        fieldCount += stream->Write(5, bulletSpeed);
        fieldCount += stream->Write(6, bulletNum);
        fieldCount += stream->Write(7, plantName);
        fieldCount += stream->Write(8, plantid);
        fieldCount += stream->Write(9, plantType);
        fieldCount += stream->Write(10, plantSuns);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__PLANTSCONFCFG_H__
