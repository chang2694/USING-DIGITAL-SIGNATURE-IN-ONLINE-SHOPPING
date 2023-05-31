#include <stdint.h>
#include <stdio.h>
#include "getPDFData.h"

uint8_t* readPDF(const char* filePath, size_t* fileSize) {
    FILE* file;
    if (fopen_s(&file, filePath, "rb") == 0) {
        // Xác định kích thước của tệp PDF
        fseek(file, 0, SEEK_END);
        *fileSize = ftell(file);
        fseek(file, 0, SEEK_SET);

        // Tạo mảng để lưu dữ liệu PDF
        uint8_t* data = (uint8_t*)malloc(*fileSize);

        // Đọc dữ liệu từ tệp PDF vào mảng byte
        fread(data, 1, *fileSize, file);

        // Đóng tệp
        fclose(file);

        return data;
    }
    else {
        printf("Failed to open file: %s\n", filePath);
        return NULL;
    }
}