#include <stddef.h>
#include <stdint.h>
#include <stdio.h>
#include "randombytes.h"
#include "sign.h"
#include "getData.h"
#include <string.h>

int main(void)
{
	int option = 3;
	if (option == 1) {
#pragma region CreateKey
		uint8_t pk[CRYPTO_PUBLICKEYBYTES];
		uint8_t sk[CRYPTO_SECRETKEYBYTES];
		crypto_sign_keypair(pk, sk);

		// Public key
		FILE* file;
		errno_t err = fopen_s(&file, "publickey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

		if (err != 0) {
			printf("Không thể mở tệp.\n");
			system("pause");
			return -1;
		}

		size_t keySize = sizeof(pk); // Kích thước của key trong byte

		size_t elementsWritten = fwrite(pk, sizeof(uint8_t), keySize, file); // Ghi key vào tệp

		if (elementsWritten != keySize) {
			printf("Lỗi khi ghi dữ liệu.\n");
			fclose(file);
			system("pause");
			return -1;
		}

		fclose(file);

		// Secret key
		err = fopen_s(&file, "secretkey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

		if (err != 0) {
			printf("Không thể mở tệp.\n");
			system("pause");
			return -1;
		}

		keySize = sizeof(sk); // Kích thước của key trong byte

		elementsWritten = fwrite(sk, sizeof(uint8_t), keySize, file); // Ghi key vào tệp

		if (elementsWritten != keySize) {
			printf("Lỗi khi ghi dữ liệu.\n");
			fclose(file);
			system("pause");
			return -1;
		}

		fclose(file);
		printf("Create keypair succeeded\n");
#pragma endregion
	}
	else {
		char fileName[50];
		fgets(fileName, 50, stdin);

		size_t newlinePos = strcspn(fileName, "\n");

		if (newlinePos < strlen(fileName)) {
			fileName[newlinePos] = '\0';
		}

		if (option == 2) {
			size_t mlen, smlen;
			uint8_t* m;

			uint8_t sk[CRYPTO_SECRETKEYBYTES];
			readKeyFromFile("secretkey.key", sk, sizeof(sk));
			if (!sk) {
				printf("Failed to read key\n");
				system("pause");
				return -1;
			}

			char filePath[50];
			strcpy_s(filePath, sizeof(filePath), fileName);
			strcat_s(filePath, sizeof(filePath), ".pdf");

			size_t dataSize;

			m = readPDF(filePath, &dataSize);
			if (!m) {
				printf("Failed to read file: %s\n", filePath);
				system("pause");
				return -1;
			}

			uint8_t* sm = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));

			crypto_sign(sm, &smlen, m, dataSize, sk);

			char signedFilePath[50];
			strcpy_s(signedFilePath, sizeof(signedFilePath), fileName);
			strcat_s(signedFilePath, sizeof(signedFilePath), "_signed.pdf");
			writeFile(signedFilePath, sm, smlen);

			char signatureFilePath[50];
			strcpy_s(signatureFilePath, sizeof(signatureFilePath), fileName);
			strcat_s(signatureFilePath, sizeof(signatureFilePath), "_signature.pdf");
			writeFile(signatureFilePath, sm, CRYPTO_BYTES);

			free(m);
			printf("Sign succeeded!");
		}
		else {
			int ret;
			size_t mlen, smlen;
			uint8_t* sig;
			uint8_t* m;

			uint8_t pk[CRYPTO_PUBLICKEYBYTES];
			readKeyFromFile("MyPublickey.key", pk, sizeof(pk));

			char signatureFilePath[50];
			strcpy_s(signatureFilePath, sizeof(signatureFilePath), fileName);
			strcat_s(signatureFilePath, sizeof(signatureFilePath), "_signature.pdf");


			size_t signatureDataSize;
			sig = readPDF(signatureFilePath, &signatureDataSize);
			if (!sig) {
				printf("Failed to read file: %s\n", signatureFilePath);
				system("pause");
				return -1;
			}
			char messageFilePath[50];
			strcpy_s(messageFilePath, sizeof(messageFilePath), fileName);
			strcat_s(messageFilePath, sizeof(messageFilePath), ".pdf");
			size_t messageDataSize;
			m = readPDF(messageFilePath, &messageDataSize);
			if (!sig) {
				printf("Failed to read file: %s\n", messageFilePath);
				system("pause");
				return -1;
			}

			ret = crypto_sign_open(sig, signatureDataSize, m, messageDataSize, pk);
			if (ret)
			{
				fprintf(stderr, "Verification failed\n");
				system("pause");
				return -1;
			}
			else {
				printf("Verification succeeded\n");
			}

			free(sig);
			free(m);
		}
	}
	system("pause");
	return 0;
}
	