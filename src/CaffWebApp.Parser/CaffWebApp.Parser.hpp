#pragma once
#include <iostream>

void emptyCaffBuffer(char*, int);
int parseCaffFile(std::ifstream&);
int writeBmpFile(std::ifstream&, int, int);
int writeBmpFileHeader(std::ofstream&, int, int);
int writeBmpFileInfoHeader(std::ofstream&, int, int);
int writeBmpFilePixels(std::ifstream&, int, int, std::ofstream&);
int littleEndianToInt(const unsigned char* buffer, int num_of_bytes);
int bigEndianToInt(const unsigned char* buffer, int num_of_bytes);
