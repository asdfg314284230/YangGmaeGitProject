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
    // 玩家子弹数
    int32_t GameZiDan;
    // 换弹时间
    int32_t HuanDanTime;
    // 怪物基础血量
    int32_t EnemyHp;
    // 怪物刷礼加成
    int32_t EnemyAddHp;
    // 主播基础攻击力
    int32_t PlayerAtk;
    // 玩家基础攻击力
    int32_t EnemyAtk;
    // 怪物刷礼物最大血量上限
    int32_t EnemyMaxHp;
    // 主播基础血量
    int32_t PlayerMaxHp;
    // 怪物放大系数（百分比）
    int32_t EnemyScale;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 2: {
                    GameZiDan = stream->ReadInt();
                    break;
                }
                case 3: {
                    HuanDanTime = stream->ReadInt();
                    break;
                }
                case 4: {
                    EnemyHp = stream->ReadInt();
                    break;
                }
                case 5: {
                    EnemyAddHp = stream->ReadInt();
                    break;
                }
                case 6: {
                    PlayerAtk = stream->ReadInt();
                    break;
                }
                case 7: {
                    EnemyAtk = stream->ReadInt();
                    break;
                }
                case 8: {
                    EnemyMaxHp = stream->ReadInt();
                    break;
                }
                case 9: {
                    PlayerMaxHp = stream->ReadInt();
                    break;
                }
                case 10: {
                    EnemyScale = stream->ReadInt();
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

        fieldCount += stream->Write(2, GameZiDan);
        fieldCount += stream->Write(3, HuanDanTime);
        fieldCount += stream->Write(4, EnemyHp);
        fieldCount += stream->Write(5, EnemyAddHp);
        fieldCount += stream->Write(6, PlayerAtk);
        fieldCount += stream->Write(7, EnemyAtk);
        fieldCount += stream->Write(8, EnemyMaxHp);
        fieldCount += stream->Write(9, PlayerMaxHp);
        fieldCount += stream->Write(10, EnemyScale);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__GAMECONFCFG_H__
