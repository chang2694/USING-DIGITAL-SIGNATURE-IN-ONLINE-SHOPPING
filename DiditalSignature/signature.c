#include <stddef.h>
#include <stdint.h>
#include <stdio.h>
#include "randombytes.h"
#include "sign.h"
#include "GetPDFData.h"

int main(void)
{
    size_t i, j;
    int ret;
    size_t mlen, smlen;
    uint8_t b;
    uint8_t *m;
    
    uint8_t pk[CRYPTO_PUBLICKEYBYTES];
    uint8_t sk[CRYPTO_SECRETKEYBYTES];

    const char* filePath = "CV_LeThiHuyenTrang.pdf";
    size_t dataSize;
    m = readPDF(filePath, &dataSize);
    if (!m) {
        printf("Failed to read file: %s\n", filePath);
        return 0;
    }

    uint8_t* sm = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));
    uint8_t* m2 = calloc(dataSize + CRYPTO_BYTES, sizeof(uint8_t));

    crypto_sign_keypair(pk, sk);
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

    randombytes((uint8_t *)&j, sizeof(j));
    do
    {
        randombytes(&b, 1);
    } while (!b);
    sm[j % (dataSize + CRYPTO_BYTES)] += b;
    ret = crypto_sign_open(m2, &mlen, sm, smlen, pk);
    if (!ret)
    {
        fprintf(stderr, "Trivial forgeries possible\n");
        return -1;
    }

    printf("CRYPTO_PUBLICKEYBYTES = %d\n", CRYPTO_PUBLICKEYBYTES);
    printf("CRYPTO_SECRETKEYBYTES = %d\n", CRYPTO_SECRETKEYBYTES);
    printf("CRYPTO_BYTES = %d\n", CRYPTO_BYTES);

    printf("\nPUBLICKEY:");
    for (i = 0; i < CRYPTO_PUBLICKEYBYTES; i++)
        printf("%x", pk[i]);
    printf("\n\nSECRECTKEY:");
    for (i = 0; i < CRYPTO_SECRETKEYBYTES; i++)
        printf("%x", sk[i]);

    free(m);
    return 0;
}
