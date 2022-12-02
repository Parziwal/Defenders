#pragma once
#include <iostream>
#include "gifanim.h"

void emptyCaffBuffer(char*, int);
int parseCaffImage(std::ifstream&, std::string&);
int writeGif(std::ifstream&, int, int, GifAnim&, GifWriter&, int);
int littleEndianToInt(const unsigned char* buffer, int num_of_bytes);
int bigEndianToInt(const unsigned char* buffer, int num_of_bytes);
