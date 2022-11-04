#ifndef __VEC2_H__
#define __VEC2_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 坐标
class Vec2 : public MessageBase
{
private:
    typedef MessageBase Super;
public:
    // X
    int32_t X;
    // Y
    int32_t Y;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 1: {
                    X = stream->ReadInt();
                    break;
                }
                case 2: {
                    Y = stream->ReadInt();
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

        fieldCount += stream->Write(1, X);
        fieldCount += stream->Write(2, Y);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__VEC2_H__
