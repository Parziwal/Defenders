#include <cstdio>
#include <cstring>
#include "CaffWebApp.Parser.hpp"

const size_t L_BUFFER = 256;

int main(int argc, char* argv[]) {
    if (argc != 2) {
        printf("Az argumentumok szama nem megfelelo");
        return 1;
    }
    FILE* fp;
    errno_t err = fopen_s(&fp, argv[1], "rb"); // b: Binaris modban olvas
    if (err != 0) {
        printf("A fajl nem nyithato meg");
        return 1;
    }
    int return_code = parseCaffFile(fp);
    fclose(fp);
    return return_code;
}

void emptyCaffBuffer(unsigned char* buffer, int length) {
    for (int i = 0; i < length; ++i)
        buffer[i] = 0;
}

int parseCaffFile(FILE* fp) {
    auto* caffBuffer = new unsigned char[L_BUFFER];
    emptyCaffBuffer(caffBuffer, L_BUFFER);
    int read = fread(caffBuffer, 1, 9, fp);
    if (read != 9) {
        printf("Tul rovid a fajl");
        return 1;
    }

    if (*caffBuffer != 1) {
        printf("A fajl nem headerrel kezdodik.");
        return 1;
    }

    int block_size_little_endian = littleEndianToInt(caffBuffer + 1, 8);
    int block_size_big_endian = bigEndianToInt(caffBuffer + 1, 8);

    int block_size;
    int (*endianConverter)(const unsigned char*, int);
    if (block_size_little_endian < block_size_big_endian) {
        endianConverter = &littleEndianToInt;
        block_size = block_size_little_endian;
    }
    else {
        endianConverter = &bigEndianToInt;
        block_size = block_size_big_endian;
    }


    if (block_size != 20) {
        printf("Az elso blokk mindig 20 bajtos.");
        return 1;
    }

    /// CAFF Header:
    /// 0-3 byte: 'CAFF'
    /// 4-11 byte: header_size
    /// 12-19 byte: num_anim
    read = fread(caffBuffer, 1, 20, fp);
    if (read != 20) {
        printf("Tul rovid a fajl");
        return 1;
    }
    /// magic 4 byte
    if (caffBuffer[0] != 'C' || caffBuffer[1] != 'A' || caffBuffer[2] != 'F' || caffBuffer[3] != 'F') {
        printf("CAFF header 'magic' nem megfelelo.");
        return 1;
    }
    /// header_size
    int header_size = (*endianConverter)(caffBuffer + 4, 8);
    if (header_size != 20) {
        printf("Az header blokk mindig 20 bajtos.");
        return 1;
    }
    /// num_anim (for annual GIF conversion in the future)
    // int num_anim = (*endianConverter)(caffBuffer + 12, 8);

    /// NEW BLOCK!!!
    emptyCaffBuffer(caffBuffer, L_BUFFER);
    read = fread(caffBuffer, 1, 9, fp);
    if (read != 9) {
        printf("Tul rovid a fajl");
        return 1;
    }
    if (caffBuffer[0] != 2) {
        printf("CAFF credits blokk kovetkezne");
        return 1;
    }
    block_size = (*endianConverter)(caffBuffer + 1, 8);
    if (block_size < 14) {
        printf("Tul kicsi a CAFF Credits blokk 'length' erteke");
        return 1;
    }
    /// CAFF Credits:
    /// 0-1 idx byte: YY (year)
    /// 2-2 idx byte: M (month)
    /// 3-3 idx byte: D (day)
    /// 4-4 idx byte: h (hour)
    /// 5-5 idx byte: m (minute)
    /// 6-13 idx byte: creator_length
    /// 14-(14+creator_length-1): creator
    emptyCaffBuffer(caffBuffer, L_BUFFER);
    read = fread(caffBuffer, 1, block_size, fp);
    if (read != block_size) {
        printf("Tul rovid a fajl");
        return 1;
    }

    // A datumot nem hasznaljuk fel
    // int creator_length = (*endianConverter)(caffBuffer + 6, 8);
    // For future:
    // int year = caffBuffer[0] | caffBuffer[1] << 8;

    //for (int i = 0; i < num_anim; ++i) {
    /// NEW BLOCK
    emptyCaffBuffer(caffBuffer, L_BUFFER);
    read = fread(caffBuffer, 1, 9, fp);
    if (read != 9) {
        printf("Tul rovid a fajl");
        return 1;
    }

    if (caffBuffer[0] != 3) {
        printf("CAFF animation blokk kovetkezne");
        return 1;
    }

    int anim_block_size = (*endianConverter)(caffBuffer + 1, 8);

    /// CAFF Animation block
    emptyCaffBuffer(caffBuffer, L_BUFFER);
    read = fread(caffBuffer, 1, 8, fp);
    if (read != 8) {
        printf("Tul rovid a fajl");
        return 1;
    }
    // if needed for the future
    //int duration = (*endianConverter)(caffBuffer, 8);

    emptyCaffBuffer(caffBuffer, L_BUFFER);
    read = fread(caffBuffer, 1, 36, fp);
    if (read != 36) {
        printf("Tul rovid a fajl");
        return 1;
    }

    if (caffBuffer[0] != 'C' || caffBuffer[1] != 'I' || caffBuffer[2] != 'F' || caffBuffer[3] != 'F') {
        printf("CIFF header 'magic' nem megfelelo.");
        return 1;
    }

    int ciff_header_size = (*endianConverter)(caffBuffer + 4, 8);
    int ciff_content_size = (*endianConverter)(caffBuffer + 12, 8);
    if (anim_block_size != 8 + ciff_header_size + ciff_content_size) {
        printf("Nem megfelelo CIFF fajl meret es CAFF animation block meret");
        return 1;
    }
    int ciff_width = (*endianConverter)(caffBuffer + 20, 8);
    int ciff_height = (*endianConverter)(caffBuffer + 28, 8);

    // nehany matematikai ellenorzes a headeren
    if (ciff_content_size % 3 != 0) {
        printf("CIFF content_size nem oszthato 3-mal");
        return 1;
    }

    int rem = ciff_content_size % (ciff_width * ciff_height);
    int quotient = ciff_content_size / (ciff_width * ciff_height);

    if (rem != 0 || quotient != 3) {
        printf("Rossz kep meret informacio a CIFF headerben");
        return 1;
    }

    emptyCaffBuffer(caffBuffer, L_BUFFER);
    read = fread(caffBuffer, 1, (size_t)(ciff_header_size)-36, fp);
    if (read != ciff_header_size - 36) {
        printf("Tul rovid a fajl");
        return 1;
    }

    writeBmpFile(fp, ciff_width, ciff_height);
    //}
    delete[] caffBuffer;
    return 0;
}

int writeBmpFile(FILE* fp, int width, int height) {
    /// CREATE and WRITE bmp file
    FILE* out;
    errno_t err = fopen_s(&out, "out.bmp", "wb");
    if (err != 0) {
        printf("Nem sikerult megnyitni a fajlt.");
        return 1;
    }

    writeBmpFileHeader(out, width, height);
    writeBmpFileInfoHeader(out, width, height);
    writeBmpFilePixels(fp, width, height, out);
    fclose(out);
    return 0;
}

int writeBmpFileHeader(FILE* out, int width, int height) {
    /// BMP HEADER
    // calculate bmp file size in bytes
    int bmp_length = width * height * 3 + 54;

    unsigned char bmp_header[14];
    emptyCaffBuffer(bmp_header, 14);
    bmp_header[0] = 'B'; bmp_header[1] = 'M';
    bmp_header[2] = bmp_length | 0x00;
    bmp_header[3] = bmp_length >> 8 | 0x00;
    bmp_header[4] = bmp_length >> 16 | 0x00;
    bmp_header[5] = bmp_length >> 24 | 0x00;
    bmp_header[10] = 54;
    bmp_header[11] = bmp_header[12] = bmp_header[13] = 0;
    fwrite(bmp_header, sizeof(bmp_header), 1, out);
    return 0;
}

int writeBmpFileInfoHeader(FILE* out, int width, int height) {
    int content_size = width * height * 3;

    unsigned char info_header[40];
    emptyCaffBuffer(info_header, 40);
    info_header[0] = 40;
    info_header[1] = info_header[2] = info_header[3] = 0;
    // width
    info_header[4] = width | 0x00;
    info_header[5] = width >> 8 | 0x00;
    info_header[6] = width >> 16 | 0x00;
    info_header[7] = width >> 24 | 0x00;
    // height
    info_header[8] = height | 0x00;
    info_header[9] = height >> 8 | 0x00;
    info_header[10] = height >> 16 | 0x00;
    info_header[11] = height >> 24 | 0x00;
    // planes (???)
    info_header[12] = 1;
    // bits per pixel
    info_header[14] = 24;
    // image size
    info_header[20] = content_size | 0x00;
    info_header[21] = content_size >> 8 | 0x00;
    info_header[22] = content_size >> 16 | 0x00;
    info_header[23] = content_size >> 24 | 0x00;

    fwrite(info_header, sizeof(info_header), 1, out);
    return 0;
}

int writeBmpFilePixels(FILE* fp, int width, int height, FILE* out) {
    /// BMP PIXEL information
    int content_size = width * height * 3;
    int window_size = width * 3;
    auto* bmp_pixel_buffer = new unsigned char[window_size];
    fseek(fp, content_size - window_size, SEEK_CUR);
    for (int i = 0; i < height; ++i) {
        emptyCaffBuffer(bmp_pixel_buffer, window_size);
        int read = fread(bmp_pixel_buffer, 1, window_size, fp);
        if (read != window_size) {
            printf("Tul rovid a fajl");
            return 1;
        }
        for (int j = 0; j < window_size; j += 3) {
            unsigned char temp = bmp_pixel_buffer[j];
            bmp_pixel_buffer[j] = bmp_pixel_buffer[j + 2];
            bmp_pixel_buffer[j + 2] = temp;
        }
        fwrite(bmp_pixel_buffer, sizeof(char), window_size, out);
        fseek(fp, -2 * window_size, SEEK_CUR);
    }

    delete[] bmp_pixel_buffer;
    return 0;
}

int littleEndianToInt(const unsigned char* buffer, int num_of_bytes) {
    int ret = 0;
    for (int i = 0; i < num_of_bytes; ++i)
        ret |= buffer[i] << i * 8;
    return ret;
}

int bigEndianToInt(const unsigned char* buffer, int num_of_bytes) {
    int ret = 0;
    for (int i = num_of_bytes - 1; i >= 0; --i) {
        ret |= buffer[i] << (num_of_bytes - i - 1) * 8;
    }
    return ret;
}