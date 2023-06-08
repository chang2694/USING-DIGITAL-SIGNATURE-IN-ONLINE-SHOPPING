#include <stddef.h>
#include <stdint.h>
#include <stdio.h>
#include "randombytes.h"
#include "sign.h"
#include "getData.h"

int main(void)
{
	size_t i, j;
	int ret;
	size_t mlen, smlen;
	uint8_t b;
	uint8_t* m;

	uint8_t pk[CRYPTO_PUBLICKEYBYTES];
	uint8_t sk[CRYPTO_SECRETKEYBYTES];

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
	

	readKeyFromFile("publickey.key", pk, sizeof(pk));
	readKeyFromFile("secretkey.key", sk, sizeof(sk));


	printf("\nPUBLICKEY:");
	for (i = 0; i < CRYPTO_PUBLICKEYBYTES; i++)
		printf("%x", pk[i]);
	printf("\n\nSECRECTKEY:");
	for (i = 0; i < CRYPTO_SECRETKEYBYTES; i++)
		printf("%x", sk[i]);

	const char* filePath = "D:\\DigitalSignature\\Dilithium\\DiditalSignature\\FilePDF\\CV_LeThiHuyenTrang.pdf";
	size_t dataSize;
	m = readPDF(filePath, &dataSize);
	if (!m) {
		printf("Failed to read file: %s\n", filePath);
		return 0;
	}

	uint8_t* sm = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));
	uint8_t* m2 = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));

	crypto_sign(sm, &smlen, m, dataSize, sk);

	ret = crypto_sign_open(m2, &mlen, sm, smlen, pk);
	if (ret)
	{
		fprintf(stderr, "Verification failed\n");
		return -1;
	}
	if (smlen != dataSize + CRYPTO_BYTES)
	{
		fprintf(stderr, "Signed message lengths wrong\n");
		return -1;
	}
	if (mlen != dataSize)
	{
		fprintf(stderr, "Message lengths wrong\n");
		return -1;
	}
	for (j = 0; j < dataSize; ++j)
	{
		if (m2[j] != m[j])
		{
			fprintf(stderr, "Messages don't match\n");
			return -1;
		}
	}


	free(m);
	return 0;
}
