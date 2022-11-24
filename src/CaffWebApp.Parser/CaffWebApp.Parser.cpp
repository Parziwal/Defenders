#include "CaffWebApp.Parser.hpp"
#include <iostream>
#include <iomanip>
#include <fstream>
#include <string>
#define ExternFunction _declspec(dllexport)

const size_t L_BUFFER = 256;

int main(int argc, char* argv[]) {
    if (argc != 2) {
        std::cout << "Az argumentumok szama nem megfelelo" << std::endl;
        return 1;
    }
    std::string filename = argv[1];
    std::ifstream caff(filename, std::ifstream::in | std::ifstream::binary);
    if (!caff.is_open()) {
        std::cout << "Nem sikerult megnyitni a bemeneti fajlt" << std::endl;
        return 1;
    }
        
    int return_code = parseCaffFile(caff, filename);
    caff.close();
    if (return_code == 0)
        std::cout << std::endl << "A generalt a kepek a beadott CAFF fajl mappajaban talalhatok BMP formatumban." << std::endl;
    return return_code;
}

void emptyCaffBuffer(char* buffer, int length) {
    for (int i = 0; i < length; ++i)
        buffer[i] = 0;
}

int parseCaffFile(std::ifstream& caff, std::string& filename) {
    auto caffBuffer = new char[L_BUFFER];

    emptyCaffBuffer(caffBuffer, L_BUFFER);

    caff.read(caffBuffer, 9);

    if (caff.gcount() != 9) {
        std::cout << "Tul rovid a fajl" << std::endl;
        return 1;
    }

    if (caffBuffer[0] != 1) {
        std::cout << "A fajl nem headerrel kezdodik." << std::endl;
        return 1;
    }

    int block_size_little_endian = littleEndianToInt((const unsigned char*)(&caffBuffer[1]), 8);
    int block_size_big_endian = bigEndianToInt((const unsigned char*)(&caffBuffer[1]), 8);

    int (*endianConverter)(const unsigned char*, int);
    if (block_size_little_endian == 20)
        endianConverter = &littleEndianToInt;
    else if (block_size_big_endian == 20) 
        endianConverter = &bigEndianToInt;
    else {
        std::cout << "Rossz header" << std::endl;
        return 1;
    }

    /// CAFF Header:
    /// 0-3 byte: 'CAFF'
    /// 4-11 byte: header_size
    /// 12-19 byte: num_anim
    caff.read(caffBuffer, 20);
    if (caff.gcount() != 20) {
        std::cout << "Tul rovid a fajl" << std::endl;
        return 1;
    }
    /// magic 4 byte
    if (caffBuffer[0] != 'C' || caffBuffer[1] != 'A' || caffBuffer[2] != 'F' || caffBuffer[3] != 'F') {
        std::cout << "CAFF header 'magic' nem megfelelo." << std::endl;
        return 1;
    }
    /// header_size
    int header_size = (*endianConverter)((const unsigned char*)(&caffBuffer[4]), 8);
    if (header_size != 20) {
        std::cout << "Az header blokk mindig 20 bajtos." << std::endl;
        return 1;
    }
    /// num_anim (for annual GIF conversion in the future)
    int num_anim = (*endianConverter)((const unsigned char*)(&caffBuffer[12]), 8);

    /// NEW BLOCK!!!
    emptyCaffBuffer(caffBuffer, L_BUFFER);
    caff.read(caffBuffer, 9);
    if (caff.gcount() != 9) {
        std::cout << "Tul rovid a fajl" << std::endl;
        return 1;
    }
    if (caffBuffer[0] != 2) {
        std::cout << "CAFF credits blokk kovetkezne" << std::endl;
        return 1;
    }
    int credits_header_size = (*endianConverter)((const unsigned char*)(&caffBuffer[1]), 8);
    if (credits_header_size < 14) {
        std::cout << "Tul kicsi a CAFF Credits blokk 'length' erteke" << std::endl;
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
    caff.read(caffBuffer, credits_header_size);
    if (caff.gcount() != credits_header_size) {
        std::cout << "Tul rovid a fajl" << std::endl;
        return 1;
    }

    int creator_length = (*endianConverter)((const unsigned char*)(&caffBuffer[6]), 8);
    int year = (*endianConverter)((const unsigned char*)(&caffBuffer[0]), 2);
    int month = caffBuffer[2] | 0x00;
    int day = caffBuffer[3] | 0x00;
    int hour = caffBuffer[4] | 0x00;
    int minute = caffBuffer[5] | 0x00;
    auto creator = new char[creator_length + 1];
    for (int i = 0; i < creator_length; i++)
        creator[i] = caffBuffer[14 + i];
    creator[creator_length] = '\0';
    

    int jump_to = 9 + 20 + 9 + credits_header_size;
    std::cout << "Meta adatok" << std::endl;
    std::cout << "CAFF keszitesi ideje: " 
        << year << '.' 
        << std::setfill('0') << std::setw(2) << month << '.' 
        << std::setfill('0') << std::setw(2) << day << ". " 
        << std::setfill('0') << std::setw(2) << hour << ':' 
        << std::setfill('0') << std::setw(2) << minute << std::endl;
    std::cout << "Kepek keszitoje: " << creator << std::endl;
    std::cout << "CIFF kepek szama a fajlban: " << num_anim << std::endl;
    delete[] creator;

    for (int i = 0; i < num_anim; ++i) {
        /// NEW BLOCK
        emptyCaffBuffer(caffBuffer, L_BUFFER);
        if (i != 0)
            caff.seekg(jump_to);
        caff.read(caffBuffer, 9);
        if (caff.gcount() != 9) {
            std::cout << "Tul rovid a fajl" << std::endl;
            return 1;
        }

        if (caffBuffer[0] != 3) {
            std::cout << "CAFF animation blokk kovetkezne" << std::endl;
            return 1;
        }

        int anim_block_size = (*endianConverter)((const unsigned char*)(&caffBuffer[1]), 8);
        jump_to += 9 + anim_block_size;

        /// CAFF Animation block
        emptyCaffBuffer(caffBuffer, L_BUFFER);
        caff.read(caffBuffer, 8);
        if (caff.gcount() != 8) {
            std::cout << "Tul rovid a fajl" << std::endl;
            return 1;
        }
        int duration = (*endianConverter)((const unsigned char*)(&caffBuffer[0]), 8);

        emptyCaffBuffer(caffBuffer, L_BUFFER);
        caff.read(caffBuffer, 36);
        if (caff.gcount() != 36) {
            std::cout << "Tul rovid a fajl" << std::endl;
            return 1;
        }

        if (caffBuffer[0] != 'C' || caffBuffer[1] != 'I' || caffBuffer[2] != 'F' || caffBuffer[3] != 'F') {
            std::cout << "CIFF header 'magic' nem megfelelo." << std::endl;
            return 1;
        }

        int ciff_header_size = (*endianConverter)((const unsigned char*)(&caffBuffer[4]), 8);
        int ciff_content_size = (*endianConverter)((const unsigned char*)(&caffBuffer[12]), 8);
        if (anim_block_size != 8 + ciff_header_size + ciff_content_size) {
            std::cout << "Nem megfelelo CIFF fajl meret es CAFF animation block meret" << std::endl;
            return 1;
        }
        int ciff_width = (*endianConverter)((const unsigned char*)(&caffBuffer[20]), 8);
        int ciff_height = (*endianConverter)((const unsigned char*)(&caffBuffer[28]), 8);

        // nehany matematikai ellenorzes a headeren
        if (ciff_content_size % 3 != 0) {
            std::cout << "CIFF content_size nem oszthato 3-mal" << std::endl;
            return 1;
        }

        int rem = ciff_content_size % (ciff_width * ciff_height);
        int quotient = ciff_content_size / (ciff_width * ciff_height);

        if (rem != 0 || quotient != 3) {
            std::cout << "Rossz kep meret informacio a CIFF headerben" << std::endl;
            return 1;
        }

        emptyCaffBuffer(caffBuffer, L_BUFFER);
        int capt_size = ciff_header_size - 36;
        caff.read(caffBuffer, capt_size);
        if (caff.gcount() != capt_size) {
            std::cout << "Tul rovid a fajl" << std::endl;
            return 1;
        }

        std::string caption(caffBuffer, capt_size);
        if (caption[capt_size - 1] != '\0') {
            std::cout << "A tagek nem jo karakterre vegzodnek" << std::endl;
            return 1;
        }
        
        size_t pos = caption.find('\n');
        if (pos == std::string::npos) {
            std::cout << "Nem talalhato a caption-t lezaro karakter" << std::endl;
            return 1;
        }

        std::cout << std::endl << (i + 1) << ". CIFF metaadatai" << std::endl;
        std::cout << "Kep merete: " << ciff_width << "x" << ciff_height << " px" << std::endl;
        std::cout << "Animacio hossza: " << duration << " ms" << std::endl;
                
        std::cout << "Kep felirata: " << caption.substr(0, pos) << std::endl;
        caption.erase(0, pos + 1);
        pos = caption.find('\0');
        if (pos == std::string::npos) {
            std::cout << "Nem talalhato a taget lezaro karakter" << std::endl;
            return 1;
        }
        std::cout << "Tagek: ";
        while (pos != std::string::npos) {
            std::cout << caption.substr(0, pos);
            caption.erase(0, pos + 1);
            pos = caption.find('\0');
            if (pos != std::string::npos)
                std::cout << ", ";
        }
        std::cout << std::endl;

        if (writeBmpFile(caff, ciff_width, ciff_height, filename, i + 1) != 0)
            return 1;
    }
    delete[] caffBuffer;
    return 0;
}

int writeBmpFile(std::ifstream& caff, int width, int height, std::string& f_name, int count) {
    /// CREATE and WRITE bmp file
    size_t lastindex = f_name.find_last_of(".");
    std::string name_without_ext;
    if (lastindex == std::string::npos)
        name_without_ext = f_name;
    else
        name_without_ext = f_name.substr(0, lastindex);
    std::string filename = name_without_ext + "_" + std::to_string(count) + ".bmp";
    std::ofstream out(filename.c_str(), std::ofstream::out | std::ios_base::binary);

    if (!out.is_open()) {
        std::cout << "Nem sikerult megnyitni a kimeneti fajlt" << std::endl;
        return 1;
    }
    writeBmpFileHeader(out, width, height);
    writeBmpFileInfoHeader(out, width, height);
    int success = writeBmpFilePixels(caff, width, height, out);
    out.close();
    if (success != 0)
        for (int i = 0; i < count; i++)
            std::remove((f_name + "_" + std::to_string(i + 1) + ".bmp").c_str());
    return success;
}

int writeBmpFileHeader(std::ofstream& out, int width, int height) {
    /// BMP HEADER
    // calculate bmp file size in bytes
    int bmp_length = width * height * 3 + 54;

    char bmp_header[14];
    emptyCaffBuffer(bmp_header, 14);
    bmp_header[0] = 'B'; bmp_header[1] = 'M';
    bmp_header[2] = bmp_length | 0x00;
    bmp_header[3] = bmp_length >> 8 | 0x00;
    bmp_header[4] = bmp_length >> 16 | 0x00;
    bmp_header[5] = bmp_length >> 24 | 0x00;
    bmp_header[10] = 54;
    bmp_header[11] = bmp_header[12] = bmp_header[13] = 0;
    out.write(bmp_header, sizeof(bmp_header));
    return 0;
}

int writeBmpFileInfoHeader(std::ofstream& out, int width, int height) {
    int content_size = width * height * 3;

    char info_header[40];
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

    out.write(info_header, sizeof(info_header));
    return 0;
}

int writeBmpFilePixels(std::ifstream& caff, int width, int height, std::ofstream& out) {
    /// BMP PIXEL information
    int content_size = width * height * 3;
    int window_size = width * 3;
    auto bmp_pixel_buffer = new char[window_size];
    caff.seekg(content_size - window_size, std::ios_base::cur);
    for (int i = 0; i < height; ++i) {
        emptyCaffBuffer(bmp_pixel_buffer, window_size);
        caff.read(bmp_pixel_buffer, window_size);
        if (caff.gcount() != window_size) {
            std::cout << "Tul rovid a fajl" << std::endl;
            return 1;
        }
        for (int j = 0; j < window_size; j += 3) {
            char temp = bmp_pixel_buffer[j];
            bmp_pixel_buffer[j] = bmp_pixel_buffer[j + 2];
            bmp_pixel_buffer[j + 2] = temp;
        }
        out.write(bmp_pixel_buffer, window_size);
        if (i + 1 != height) {
            caff.seekg(-2 * window_size, std::ios_base::cur);
        }
    }
    delete[] bmp_pixel_buffer;
    return 0;
}

int littleEndianToInt(const unsigned char* buffer, int num_of_bytes) {
    unsigned int ret = 0;
    for (int i = 0; i < num_of_bytes; ++i)
        ret |= buffer[i] << i * 8;
    return ret;
}

int bigEndianToInt(const unsigned char* buffer, int num_of_bytes) {
    unsigned int ret = 0;
    for (int i = num_of_bytes - 1; i >= 0; --i) {
        ret |= buffer[i] << (num_of_bytes - i - 1) * 8;
    }
    return ret;
}

extern "C" {
    ExternFunction int AddNumber(int a, int b) {
        return a + b;
    }
}