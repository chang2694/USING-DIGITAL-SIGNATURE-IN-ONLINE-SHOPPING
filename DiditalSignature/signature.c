#include <stddef.h>
#include <stdint.h>
#include <stdio.h>
#include "randombytes.h"
#include "sign.h"
#include "getData.h"
#include <string.h>

int main(void)
{
#pragma region CreateKey
	//crypto_sign_keypair(pk, sk);

	//// Public key
	//FILE* file;
	//errno_t err = fopen_s(&file, "publickey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

	//if (err != 0) {
	//	printf("Không thể mở tệp.\n");
	//	return 1;
	//}

	//size_t keySize = sizeof(pk); // Kích thước của key trong byte

	//size_t elementsWritten = fwrite(pk, sizeof(uint8_t), keySize, file); // Ghi key vào tệp

	//if (elementsWritten != keySize) {
	//	printf("Lỗi khi ghi dữ liệu.\n");
	//	fclose(file);
	//	return 1;
	//}

	//fclose(file);
	//printf("Đã ghi key vào tệp publickey.key.\n");

	//// Secret key
	//err = fopen_s(&file, "secretkey.key", "wb"); // Mở tệp để ghi dữ liệu nhị phân (binary) với fopen_s

	//if (err != 0) {
	//	printf("Không thể mở tệp.\n");
	//	return 1;
	//}

	//keySize = sizeof(sk); // Kích thước của key trong byte

	//elementsWritten = fwrite(sk, sizeof(uint8_t), keySize, file); // Ghi key vào tệp

	//if (elementsWritten != keySize) {
	//	printf("Lỗi khi ghi dữ liệu.\n");
	//	fclose(file);
	//	return 1;
	//}

	//fclose(file);
	//printf("Đã ghi key vào tệp secretkey.key.\n");
#pragma endregion

	char fileName[5];
	fgets(fileName, 5, stdin);

	char filePath[100];
	strcpy_s(filePath, sizeof(filePath), "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\FilePDF\\");
	strcat_s(filePath, sizeof(filePath), fileName);

	int option = 2;
	if (option == 1) {
		size_t mlen, smlen;
		uint8_t* m;

		uint8_t sk[CRYPTO_SECRETKEYBYTES];
		readKeyFromFile("secretkey.key", sk, sizeof(sk));


		strcat_s(filePath, sizeof(filePath), ".pdf");

		size_t dataSize;
		m = readPDF(filePath, &dataSize);
		if (!m) {
			printf("Failed to read file: %s\n", filePath);
			return 0;
		}

		uint8_t* sm = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));

		crypto_sign(sm, &smlen, m, dataSize, sk);

		char signedFilePath[100];
		strcpy_s(signedFilePath, sizeof(signedFilePath), "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\FilePDF\\");
		strcat_s(signedFilePath, sizeof(filePath), fileName);
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

	strcat_s(filePath, sizeof(filePath), "_signed.pdf");
	size_t dataSize;
	sm = readPDF(filePath, &dataSize);
	if (!sm) {
		printf("Failed to read file: %s\n", filePath);
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
	system("pause");
	return 0;

}