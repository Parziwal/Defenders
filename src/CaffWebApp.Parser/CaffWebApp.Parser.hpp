#pragma once
#include <cstdio>

void emptyCaffBuffer(unsigned char*, int);
int parseCaffFile(FILE* fp);
int writeBmpFile(FILE*, int, int);
int writeBmpFileHeader(FILE*, int, int);
int writeBmpFileInfoHeader(FILE*, int, int);
int writeBmpFilePixels(FILE* fp, int, int, FILE* out);
int littleEndianToInt(const unsigned char* buffer, int num_of_bytes);
int bigEndianToInt(const unsigned char* buffer, int num_of_bytes);
