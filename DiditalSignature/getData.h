#include <stdint.h>
#include <stdio.h>

uint8_t* readPDF(const char* filePath, size_t* fileSize);

void readKeyFromFile(const char* filename, uint8_t* keyData, size_t keySize);

void writeFile(const char* filename, uint8_t* Data, size_t Size);