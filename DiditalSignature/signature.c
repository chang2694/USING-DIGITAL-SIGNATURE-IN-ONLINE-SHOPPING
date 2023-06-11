#include <stddef.h>
#include <stdint.h>
#include <stdio.h>
#include "randombytes.h"
#include "sign.h"
#include "getData.h"
#include <string.h>

int main(void)
{
	int option = 1;
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
			return 1;
		}

		size_t keySize = sizeof(pk); // Kích thước của key trong byte

		size_t elementsWritten = fwrite(pk, sizeof(uint8_t), keySize, file); // Ghi key vào tệp

		if (elementsWritten != keySize) {
			printf("Lỗi khi ghi dữ liệu.\n");
			fclose(file);
			return 1;
		}

		fclose(file);

		// Secret key
		err = fopen_s(&file, "secretkey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

		if (err != 0) {
			printf("Không thể mở tệp.\n");
			return 1;
		}

		keySize = sizeof(sk); // Kích thước của key trong byte

		elementsWritten = fwrite(sk, sizeof(uint8_t), keySize, file); // Ghi key vào tệp

		if (elementsWritten != keySize) {
			printf("Lỗi khi ghi dữ liệu.\n");
			fclose(file);
			return 1;
		}

		fclose(file);
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

			char filePath[50];
			strcpy_s(filePath, sizeof(filePath), fileName);
			strcat_s(filePath, sizeof(filePath), ".pdf");

			size_t dataSize;

			m = readPDF(filePath, &dataSize);
			if (!m) {
				printf("Failed to read file: %s\n", filePath);
				return 0;
			}

			uint8_t* sm = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));

			crypto_sign(sm, &smlen, m, dataSize, sk);

			char signedFilePath[50];
			strcpy_s(signedFilePath, sizeof(signedFilePath), fileName);
			strcat_s(signedFilePath, sizeof(signedFilePath), "_signed.pdf");
			writeFile(signedFilePath, sm, smlen);
			free(m);
		}
		else {
			int ret;
			size_t mlen, smlen;
			uint8_t* sm;

			uint8_t pk[CRYPTO_PUBLICKEYBYTES];
			readKeyFromFile("publickey.key", pk, sizeof(pk));

			char signedFilePath[50];
			strcpy_s(signedFilePath, sizeof(signedFilePath), fileName);
			strcat_s(signedFilePath, sizeof(signedFilePath), "_signed.pdf");
			size_t dataSize;
			sm = readPDF(signedFilePath, &dataSize);
			if (!sm) {
				printf("Failed to read file: %s\n", signedFilePath);
				return 0;
			}
			mlen = dataSize - CRYPTO_BYTES;
			smlen = dataSize;

			uint8_t* m = calloc(mlen, sizeof(uint8_t));
			for (int i = 0; i < mlen; ++i)
				m[mlen - 1 - i] = sm[CRYPTO_BYTES + mlen - 1 - i];

			ret = crypto_sign_open(&mlen, sm, smlen, pk);
			if (ret)
			{
				fprintf(stderr, "Verification failed\n");
				return -1;
			}

			free(sm);
		}
	}
		system("pause");
		return 0;

	
}