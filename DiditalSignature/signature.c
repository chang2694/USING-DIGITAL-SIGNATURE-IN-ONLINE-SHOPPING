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
		errno_t err = fopen_s(&file, "MyPublickey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

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
		err = fopen_s(&file, "MySecretkey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

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
		char* fileName = "sign.txt";

		if (option == 2) {
			size_t mlen, smlen;
			uint8_t* m;

			uint8_t sk[CRYPTO_SECRETKEYBYTES];
			int check = readKeyFromFile("MySecretkey.key", sk, sizeof(sk));
			if (check != 0) {
				return -1;
			}

			size_t dataSize;

			m = readPDF(fileName, &dataSize);
			if (!m) {
				return -1;
			}

			uint8_t* sm = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));

			crypto_sign(sm, &smlen, m, dataSize, sk);

			char* signatureFilePath = "signature.txt";
			writeFile(signatureFilePath, sm, CRYPTO_BYTES);


			free(m);
		}
		else {
			int ret;
			size_t mlen, smlen;
			uint8_t* sig;
			uint8_t* m;
			
			uint8_t pk[CRYPTO_PUBLICKEYBYTES];
			int check = readKeyFromFile("publickey.key", pk, sizeof(pk));
			if (check != 0) {
				return -1;
			}

			char* signatureFilePath = "signature.txt";

			size_t signatureDataSize;
			sig = readPDF(signatureFilePath, &signatureDataSize);
			if (!sig) {
				return -1;
			}
			size_t messageDataSize;
			m = readPDF(fileName, &messageDataSize);
			if (!sig) {
				return -1;
			}

			ret = crypto_sign_open(sig, signatureDataSize, m, messageDataSize, pk);
			if (ret)
			{
				return -1;
			}

			free(sig);
			free(m);
		}
	}
	return 0;
}
	