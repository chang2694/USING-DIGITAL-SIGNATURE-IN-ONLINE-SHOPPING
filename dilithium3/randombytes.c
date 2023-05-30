#include <stddef.h>
#include <stdint.h>
#include <stdlib.h>
#include "randombytes.h"

#include <windows.h>
#include <wincrypt.h>

void randombytes(uint8_t *out, size_t outlen)
{
  HCRYPTPROV ctx;
  size_t len;

  if (!CryptAcquireContext(&ctx, NULL, NULL, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
    abort();

  while (outlen > 0)
  {
    len = (outlen > 1048576) ? 1048576 : outlen;
    if (!CryptGenRandom(ctx, len, (BYTE *)out))
      abort();

    out += len;
    outlen -= len;
  }

  if (!CryptReleaseContext(ctx, 0))
    abort();
}