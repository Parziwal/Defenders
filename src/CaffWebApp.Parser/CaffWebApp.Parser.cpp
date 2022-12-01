#include "CaffWebApp.Parser.hpp"
#include "gifanim.h"
#include <iostream>
#include <iomanip>
#include <fstream>
#include <string>
#include <cmath>

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
    std::string outputFileName = filename + ".gif";
    int return_code = parseCaffImage(caff, filename, outputFileName);
    caff.close();
    if (return_code == 0)
        std::cout << std::endl << "A generalt GIF a beadott CAFF fajl mappajaban talalhato." << std::endl;
    return return_code;
}

void emptyCaffBuffer(char* buffer, int length) {
    for (int i = 0; i < length; ++i)
        buffer[i] = 0;
}

int parseCaffImage(std::ifstream& caff, std::string& filename, std::string& outputFileName) {
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

    std::string outputMetaDataFileName = outputFileName + ".metadata";
    std::ofstream meta_out(outputMetaDataFileName.c_str(), std::ofstream::out);

    meta_out << num_anim << std::endl;

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

    meta_out << year << '-'
        << std::setfill('0') << std::setw(2) << month << '-'
        << std::setfill('0') << std::setw(2) << day << " "
        << std::setfill('0') << std::setw(2) << hour << ':'
        << std::setfill('0') << std::setw(2) << minute << std::endl;
    meta_out << creator << std::endl;
    delete[] creator;

    GifAnim gifAnim;
    GifWriter gifWriter;

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
        if (i == 0)
            gifAnim.GifBegin(&gifWriter, outputFileName.c_str(), ciff_width, ciff_height, duration, num_anim, 8, false);

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
        meta_out << ciff_width << "x" << ciff_height << std::endl;
        std::cout << "Animacio hossza: " << duration << " ms" << std::endl;
        meta_out << duration << std::endl;

        std::cout << "Kep felirata: " << caption.substr(0, pos) << std::endl;
        meta_out << caption.substr(0, pos) << std::endl;
        caption.erase(0, pos + 1);
        pos = caption.find('\0');
        if (pos == std::string::npos) {
            std::cout << "Nem talalhato a taget lezaro karakter" << std::endl;
            return 1;
        }
        std::cout << "Tagek: ";
        while (pos != std::string::npos) {
            std::cout << caption.substr(0, pos);
            meta_out << caption.substr(0, pos);
            caption.erase(0, pos + 1);
            pos = caption.find('\0');
            if (pos != std::string::npos) {
                std::cout << ", ";
                meta_out << ",";
            }
        }
        std::cout << std::endl;
        meta_out << std::endl;

        if (writeGif(caff, ciff_width, ciff_height, gifAnim, gifWriter, duration) != 0) {
            gifAnim.GifEnd(&gifWriter);
            std::remove(outputFileName.c_str());
            return 1;
        }
    }
    delete[] caffBuffer;
    gifAnim.GifEnd(&gifWriter);
    return 0;
}

int writeGif(std::ifstream& caff, int width, int height, GifAnim& anim, GifWriter& writer, int duration) {
    int in_window_size = width * 3;
    int out_content_size = width * height * 4;
    int out_window_size = width * 4;
    auto image = new uint8_t[out_content_size];
    auto window = new char[in_window_size];
    emptyCaffBuffer(window, in_window_size);

    for (int i = 0; i < height; ++i) {
        caff.read(window, in_window_size);
        if (caff.gcount() != in_window_size) {
            std::cout << "Tul rovid a fajl" << std::endl;
            return 1;
        }
        for (int j = 0; j < width; ++j) {
            image[i * out_window_size + j * 4 + 0] = window[j * 3 + 0];
            image[i * out_window_size + j * 4 + 1] = window[j * 3 + 1];
            image[i * out_window_size + j * 4 + 2] = window[j * 3 + 2];
            image[i * out_window_size + j * 4 + 3] = 255;
        }
    }

    anim.GifWriteFrame(&writer, image, width, height, duration);

    delete[] window;
    delete[] image;
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

# if defined(_MSC_VER)
#define ExternFunction _declspec(dllexport)

extern "C" {
    ExternFunction int ParseCaffFile(const char* filePath, const char* outputPath) {
        std::ifstream caff(filePath, std::ifstream::in | std::ifstream::binary);
        if (!caff.is_open()) {
            std::cout << "Nem sikerult megnyitni a bemeneti fajlt" << std::endl;
            return 1;
        }
        std::string file_in = filePath;
        std::string file_out = outputPath;

        int return_code = parseCaffImage(caff, file_in, file_out);
        caff.close();
        if (return_code != 0) {
            std::string meta_file = file_out + ".metadata";
            std::remove(outputPath);
            std::remove(meta_file.c_str());
        }
        return return_code;
    }
}

# endif