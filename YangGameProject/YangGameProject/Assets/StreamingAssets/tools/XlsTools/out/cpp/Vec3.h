#ifndef __VEC3_H__
#define __VEC3_H__

#include <vector>
#include <string>
#include <stdint.h>
#include "ProtoBuffer.h"
#include "ProtoStream.h"
#include "ProtoType.h"
#include "MessageBase.h"
#include "ConfigBase.h"
// 坐标
class Vec3 : public MessageBase
{
private:
    typedef MessageBase Super;
public:
    // A
    int32_t A;
    // B
    int32_t B;
    // C
    int32_t C;

    void Decode(ProtoStream *stream) override {
        Super::Decode(stream);
        int tagAndType = 0;
        int fieldCount = stream->ReadFixedShort();

        while ( fieldCount-- > 0 ) {
            tagAndType = stream->ReadInt();

            switch ((tagAndType >> TAG_TYPE_BITS)) {
                case 1: {
                    A = stream->ReadInt();
                    break;
                }
                case 2: {
                    B = stream->ReadInt();
                    break;
                }
                case 3: {
                    C = stream->ReadInt();
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

        fieldCount += stream->Write(1, A);
        fieldCount += stream->Write(2, B);
        fieldCount += stream->Write(3, C);

        if(fieldCount > 0)
            stream->WriteFixedShort(fieldCount, pos);
    }
};

#endif //__VEC3_H__
